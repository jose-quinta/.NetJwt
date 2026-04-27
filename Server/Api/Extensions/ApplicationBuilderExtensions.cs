using Server.Api.Middleware;

namespace Server.Api.Extensions {
    public static class ApplicationBuilderExtensions {
        public static IApplicationBuilder UseLoggingMiddleware(this IApplicationBuilder app) {
            return app.UseMiddleware<ErrorHandlingMiddleware>();
        }
    }
}