using Microsoft.EntityFrameworkCore;
using Server.Domain.Entities;
using Server.Infrastructure.Persistence;

namespace Server.Tests.UnitTests.Repositories {
    public class RepositoryTests {
        private ApplicationDbContext CreateContext() {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            return new ApplicationDbContext(options);
        }

        private User CreateTestUser() {
            return new User {
                Username = "testuser",
                Email = "test@example.com",
                Role = "User",
                PasswordHash = new byte[64],
                PasswordSalt = new byte[64]
            };
        }

        [Fact]
        public async Task AddAsync_ShouldAddEntity() {
            using var context = CreateContext();
            var repository = new Repository<User>(context);

            var user = CreateTestUser();

            await repository.AddAsync(user);
            await context.SaveChangesAsync();

            var addedUser = await context.Users.FirstOrDefaultAsync(u => u.Username == "testuser");
            
            Assert.NotNull(addedUser);
            Assert.Equal("test@example.com", addedUser.Email);
        }

        [Fact]
        public async Task GetAsync_ShouldReturnEntityById() {
            using var context = CreateContext();
            var repository = new Repository<User>(context);

            var user = CreateTestUser();
            
            await context.Users.AddAsync(user);
            await context.SaveChangesAsync();

            var result = await repository.GetAsync(user.Id);

            Assert.NotNull(result);
            Assert.Equal("testuser", result.Username);
        }

        [Fact]
        public async Task FirstOrDefaultAsync_ShouldReturnMatchingEntity() {
            using var context = CreateContext();
            var repository = new Repository<User>(context);

            await context.Users.AddRangeAsync(
                CreateTestUser(),
                CreateTestUser()
            );
            await context.SaveChangesAsync();

            var result = await repository.FirstOrDefaultAsync(u => u.Username == "testuser");

            Assert.NotNull(result);
        }

        [Fact]
        public async Task FindAsync_ShouldReturnMatchingEntities() {
            using var context = CreateContext();
            var repository = new Repository<User>(context);

            await context.Users.AddRangeAsync(
                CreateTestUser(),
                CreateTestUser(),
                CreateTestUser()
            );
            await context.SaveChangesAsync();

            var result = await repository.FindAsync(u => u.Username.StartsWith("test"));

            Assert.Equal(3, result.Count());
        }

        [Fact]
        public async Task UpdateAsync_ShouldModifyEntity() {
            using var context = CreateContext();
            var repository = new Repository<User>(context);

            var user = CreateTestUser();
            
            await context.Users.AddAsync(user);
            await context.SaveChangesAsync();

            user.Username = "updateduser";
            await repository.UpdateAsync(user);
            await context.SaveChangesAsync();

            var updatedUser = await context.Users.FirstOrDefaultAsync(u => u.Id == user.Id);
            
            Assert.NotNull(updatedUser);
            Assert.Equal("updateduser", updatedUser.Username);
        }

        [Fact]
        public async Task DeleteAsync_ShouldRemoveEntity() {
            using var context = CreateContext();
            var repository = new Repository<User>(context);

            var user = CreateTestUser();
            
            await context.Users.AddAsync(user);
            await context.SaveChangesAsync();

            await repository.DeleteAsync(user);
            await context.SaveChangesAsync();

            var result = await context.Users.FirstOrDefaultAsync(u => u.Id == user.Id);
            
            Assert.Null(result);
        }
    }
}