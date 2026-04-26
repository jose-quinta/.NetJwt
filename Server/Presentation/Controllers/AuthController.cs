using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Server.Application.DTOs;
using Server.Application.Interfaces;
using Server.Application.Validators;
using Server.Domain.Entities;

namespace Server.Presentation.Controllers {
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase {
        private readonly IAuthService _authService;
        private readonly IUnitOfWork _unitOfWork;

        public AuthController(IAuthService authService, IUnitOfWork unitOfWork) {
            _authService = authService;
            _unitOfWork = unitOfWork;
        }

        [HttpGet("Name"), Authorize]
        public ActionResult<object> GetName() {
            var name = User.FindFirstValue(ClaimTypes.Name);
            return Ok(new { name });
        }

        [HttpGet("Role"), Authorize]
        public ActionResult<object> GetRole() {
            var role = User.FindFirstValue(ClaimTypes.Role);
            return Ok(new { role });
        }

        [HttpPost("Register")]
        public async Task<ActionResult<object>> Register([FromBody] RegisterRequest request) {
            var validator = new RegisterRequestValidator();
            var validationResult = validator.Validate(request);
            if (!validationResult.IsValid)
                return BadRequest(validationResult.Errors.Select(e => e.ErrorMessage));

            var (user, error) = await _authService.RegisterAsync(request);
            if (error != null)
                return BadRequest(new { error });

            return Ok(new UserResponse {
                Id = user!.Id,
                Username = user.Username,
                Email = user.Email,
                Role = user.Role
            });
        }

        [HttpPost("Login")]
        public async Task<ActionResult<object>> Login([FromBody] LoginRequest request) {
            var validator = new LoginRequestValidator();
            var validationResult = validator.Validate(request);
            if (!validationResult.IsValid)
                return BadRequest(validationResult.Errors.Select(e => e.ErrorMessage));

            var (response, error) = await _authService.LoginAsync(request);
            if (error != null)
                return Unauthorized(new { error });

            return Ok(response);
        }

        [HttpPost("RefreshToken")]
        public async Task<ActionResult<object>> RefreshToken([FromBody] RefreshTokenRequest request) {
            var validator = new RefreshTokenRequestValidator();
            var validationResult = validator.Validate(request);
            if (!validationResult.IsValid)
                return BadRequest(validationResult.Errors.Select(e => e.ErrorMessage));

            var (response, error) = await _authService.RefreshTokenAsync(request);
            if (error != null)
                return Unauthorized(new { error });

            return Ok(response);
        }

        [HttpPost("Logout"), Authorize]
        public async Task<ActionResult> Logout() {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId != null && int.TryParse(userId, out var id)) {
                var refreshRepo = _unitOfWork.GetRepository<RefreshToken>();
                var refreshTokens = await refreshRepo.FindAsync(rt => rt.UserId == id && rt.Revoked == null);
                foreach (var rt in refreshTokens) {
                    rt.Revoked = DateTime.UtcNow;
                    rt.RevokedByIp = HttpContext.Connection.RemoteIpAddress?.ToString();
                    await refreshRepo.UpdateAsync(rt);
                }
                await _unitOfWork.SaveAsync();
            }
            return Ok(new { message = "Logout exitoso" });
        }
    }
}