using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Server.Application.Interfaces;
using System.Security.Claims;

namespace Server.Api.Filters {
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    public class PermissionAttribute : Attribute, IAsyncAuthorizationFilter {
        private readonly string _permission;

        public PermissionAttribute(string permission) {
            _permission = permission;
        }

        public Task OnAuthorizationAsync(AuthorizationFilterContext context) {
            var user = context.HttpContext.User;
            
            if (!user.Identity?.IsAuthenticated ?? true) {
                context.Result = new UnauthorizedObjectResult(new { message = "Unauthorized" });
                return Task.CompletedTask;
            }

            var hasPermission = user.HasClaim(c => c.Type == "Permission" && c.Value == _permission);

            if (!hasPermission) {
                var role = user.FindFirst(ClaimTypes.Role)?.Value;
                
                if (role == "Admin") {
                    return Task.CompletedTask;
                }
                
                context.Result = new ForbidResult();
            }

            return Task.CompletedTask;
        }
    }
}