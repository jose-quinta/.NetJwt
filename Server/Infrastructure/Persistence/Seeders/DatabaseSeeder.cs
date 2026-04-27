using Server.Application.Interfaces;
using Server.Domain.Entities;

namespace Server.Infrastructure.Persistence {
    public class DatabaseSeeder {
        private readonly ApplicationDbContext _context;
        private readonly IUnitOfWork _unitOfWork;

        public DatabaseSeeder(ApplicationDbContext context, IUnitOfWork unitOfWork) {
            _context = context;
            _unitOfWork = unitOfWork;
        }

        public async Task SeedAsync() {
            await SeedAdminUserAsync();
        }

        private async Task SeedAdminUserAsync() {
            var userRepo = _unitOfWork.GetRepository<User>();
            
            var existingAdmin = await userRepo.FirstOrDefaultAsync(u => u.Email == "admin@example.com");
            if (existingAdmin != null) return;

            using var hmac = new System.Security.Cryptography.HMACSHA512();
            var passwordSalt = hmac.Key;
            var passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes("Admin123!"));

            var admin = new User {
                Username = "admin",
                Email = "admin@example.com",
                Role = "Admin",
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt,
                CreatedAt = DateTime.UtcNow
            };

            await userRepo.AddAsync(admin);
            await _unitOfWork.SaveAsync();
        }
    }
}