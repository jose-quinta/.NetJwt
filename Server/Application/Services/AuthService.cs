using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;
using Server.Application.DTOs;
using Server.Application.Interfaces;
using Server.Domain.Entities;

namespace Server.Application.Services {
    public class AuthService : IAuthService {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfigService _configService;
        private readonly IUserContext _userContext;

        public AuthService(IUnitOfWork unitOfWork, IConfigService configService, IUserContext userContext) {
            _unitOfWork = unitOfWork;
            _configService = configService;
            _userContext = userContext;
        }

        public async Task<(User? User, string? Error)> RegisterAsync(RegisterRequest request) {
            var userRepo = _unitOfWork.GetRepository<User>();

            if (await userRepo.FirstOrDefaultAsync(u => u.Email == request.Email) != null)
                return (null, "El email ya está registrado");

            if (await userRepo.FirstOrDefaultAsync(u => u.Username == request.Username) != null)
                return (null, "El username ya está en uso");

            CreatePasswordHash(request.Password, out byte[] passwordHash, out byte[] passwordSalt);

            var user = new User {
                Username = request.Username,
                Email = request.Email,
                Role = "User",
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt
            };

            await userRepo.AddAsync(user);
            await _unitOfWork.SaveAsync();

            return (user, null);
        }

        public async Task<(AuthResponse? Response, string? Error)> LoginAsync(LoginRequest request) {
            var userRepo = _unitOfWork.GetRepository<User>();
            var rtRepo = _unitOfWork.GetRepository<RefreshToken>();

            var user = await userRepo.FirstOrDefaultAsync(u => u.Email == request.Email);
            if (user == null)
                return (null, "Credenciales inválidas");

            if (!VerifyPasswordHash(request.Password, user.PasswordHash, user.PasswordSalt))
                return (null, "Credenciales inválidas");

            var token = CreateToken(user);
            var refreshToken = CreateRefreshToken();

            var rt = new RefreshToken {
                UserId = user.Id,
                Token = refreshToken,
                Expires = DateTime.UtcNow.AddDays(7),
                CreatedByIp = GetClientIpAddress()
            };
            await rtRepo.AddAsync(rt);
            await _unitOfWork.SaveAsync();

            return (new AuthResponse { Token = token, RefreshToken = refreshToken }, null);
        }

        public async Task<(AuthResponse? Response, string? Error)> RefreshTokenAsync(RefreshTokenRequest request) {
            var userRepo = _unitOfWork.GetRepository<User>();
            var rtRepo = _unitOfWork.GetRepository<RefreshToken>();

            var refreshToken = await rtRepo.FirstOrDefaultAsync(rt => rt.Token == request.RefreshToken);
            if (refreshToken == null)
                return (null, "Refresh token inválido");

            if (refreshToken.Revoked != null || refreshToken.Expires < DateTime.UtcNow)
                return (null, "Refresh token expirado");

            refreshToken.Revoked = DateTime.UtcNow;
            refreshToken.RevokedByIp = GetClientIpAddress();
            await rtRepo.UpdateAsync(refreshToken);

            var user = await userRepo.GetAsync(refreshToken.UserId);
            var newToken = CreateToken(user!);
            var newRefreshToken = CreateRefreshToken();

            var newRt = new RefreshToken {
                UserId = refreshToken.UserId,
                Token = newRefreshToken,
                Expires = DateTime.UtcNow.AddDays(7),
                CreatedByIp = GetClientIpAddress()
            };
            await rtRepo.AddAsync(newRt);
            await _unitOfWork.SaveAsync();

            return (new AuthResponse { Token = newToken, RefreshToken = newRefreshToken }, null);
        }

        private string CreateToken(User user) {
            var claims = new List<Claim> {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(
                _configService.GetValue("AppSettings:Token")));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(15),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private string CreateRefreshToken() {
            var randomNumber = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt) {
            using var hmac = new HMACSHA512();
            passwordSalt = hmac.Key;
            passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
        }

        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt) {
            using var hmac = new HMACSHA512(passwordSalt);
            var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            return computedHash.SequenceEqual(passwordHash);
        }

        private string GetClientIpAddress() => _userContext.GetClientIp();
    }
}