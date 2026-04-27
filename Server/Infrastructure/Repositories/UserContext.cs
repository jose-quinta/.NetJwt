using Microsoft.AspNetCore.Http;
using Server.Application.Interfaces;

namespace Server.Infrastructure.Repositories {
    public class UserContext : IUserContext {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserContext(IHttpContextAccessor httpContextAccessor) {
            _httpContextAccessor = httpContextAccessor;
        }

        public string GetClientIp() =>
            _httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString() ?? "unknown";
    }
}