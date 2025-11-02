using Microsoft.AspNetCore.Builder;
using MonitoringSystem.Observability.Middleware;

namespace MonitoringSystem.Observability.Extensions
{
    /// <summary>
    /// Extension methods for setting up global error handling in an ASP.NET Core application.
    /// </summary>
    public static class GlobalErrorHandlingExtensions
    {
        /// <summary>
        /// Adds the GlobalErrorHandlingMiddleware to the specified IApplicationBuilder.
        /// </summary>
        /// <param name="builder">The IApplicationBuilder to add the middleware to.</param>
        /// <returns>The IApplicationBuilder so that additional calls can be chained.</returns>
        public static IApplicationBuilder UseGlobalErrorHandling(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<GlobalErrorHandlingMiddleware>();
        }
    }
}
