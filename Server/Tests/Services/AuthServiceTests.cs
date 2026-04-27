using System.Linq.Expressions;
using Moq;
using Server.Application.DTOs;
using Server.Application.Interfaces;
using Server.Application.Services;
using Server.Domain.Entities;

namespace Server.Tests.UnitTests.Services {
    public class AuthServiceTests {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IConfigService> _mockConfigService;
        private readonly Mock<IUserContext> _mockUserContext;
        private readonly AuthService _authService;

        public AuthServiceTests() {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockConfigService = new Mock<IConfigService>();
            _mockUserContext = new Mock<IUserContext>();
            _authService = new AuthService(
                _mockUnitOfWork.Object,
                _mockConfigService.Object,
                _mockUserContext.Object
            );
        }

        [Fact]
        public async Task RegisterAsync_WithDuplicateEmail_ReturnsError() {
            var request = new RegisterRequest {
                Username = "testuser",
                Email = "test@example.com",
                Password = "password123"
            };

            var mockRepo = new Mock<IRepository<User>>();
            mockRepo.Setup(r => r.FirstOrDefaultAsync(It.IsAny<Expression<Func<User, bool>>>()))
                .ReturnsAsync(new User { Email = "test@example.com" });

            _mockUnitOfWork.Setup(u => u.GetRepository<User>()).Returns(mockRepo.Object);
            _mockConfigService.Setup(c => c.GetValue(It.IsAny<string>())).Returns("test-token-key-12345678901234567890");

            var (user, error) = await _authService.RegisterAsync(request);

            Assert.Null(user);
            Assert.Equal("El email ya está registrado", error);
        }

        [Fact]
        public async Task RegisterAsync_WithValidData_ReturnsUser() {
            var request = new RegisterRequest {
                Username = "newuser",
                Email = "new@example.com",
                Password = "password123"
            };

            var mockRepo = new Mock<IRepository<User>>();
            
            User? capturedUser = null;
            mockRepo.Setup(r => r.FirstOrDefaultAsync(It.IsAny<Expression<Func<User, bool>>>()))
                .ReturnsAsync((User?)null);
            mockRepo.Setup(r => r.AddAsync(It.IsAny<User>()))
                .Callback<User>(u => capturedUser = u)
                .Returns(() => Task.FromResult(capturedUser!));

            _mockUnitOfWork.Setup(u => u.GetRepository<User>()).Returns(mockRepo.Object);
            _mockUnitOfWork.Setup(u => u.SaveAsync()).Returns(Task.CompletedTask);
            _mockConfigService.Setup(c => c.GetValue(It.IsAny<string>())).Returns("test-token-key-12345678901234567890");

            var (user, error) = await _authService.RegisterAsync(request);

            Assert.NotNull(user);
            Assert.Null(error);
            Assert.Equal("newuser", user.Username);
            Assert.Equal("new@example.com", user.Email);
        }

        [Fact]
        public async Task LoginAsync_WithInvalidEmail_ReturnsError() {
            var request = new LoginRequest {
                Email = "nonexistent@example.com",
                Password = "password123"
            };

            var mockRepo = new Mock<IRepository<User>>();
            mockRepo.Setup(r => r.FirstOrDefaultAsync(It.IsAny<Expression<Func<User, bool>>>()))
                .ReturnsAsync((User?)null);

            _mockUnitOfWork.Setup(u => u.GetRepository<User>()).Returns(mockRepo.Object);

            var (response, error) = await _authService.LoginAsync(request);

            Assert.Null(response);
            Assert.Equal("Credenciales inválidas", error);
        }
    }
}