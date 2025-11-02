using Microsoft.AspNetCore.Builder;
using Serilog;
using MonitoringSystem.Observability.Configuration;
using MonitoringSystem.Observability.Middleware;

namespace MonitoringSystem.Observability.Extensions;

/// <summary>
/// Extension methods for configuring observability middleware
/// </summary>
public static class ObservabilityApplicationBuilderExtensions
{
    /// <summary>
    /// Adds observability middleware to the application pipeline
    /// </summary>
    public static IApplicationBuilder UseObservability(this IApplicationBuilder app)
    {
        // Add correlation ID middleware
        app.UseMiddleware<CorrelationIdMiddleware>();

        // Get options from DI
        var options = app.ApplicationServices.GetService(typeof(ObservabilityOptions)) as ObservabilityOptions;

        // Add Serilog request logging if enabled
        if (options?.Logging.EnableRequestLogging ?? true)
        {
            app.UseSerilogRequestLogging(opts =>
            {
                opts.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
                {
                    diagnosticContext.Set("RequestHost", httpContext.Request.Host.Value);
                    diagnosticContext.Set("RequestScheme", httpContext.Request.Scheme);
                    diagnosticContext.Set("RemoteIpAddress", httpContext.Connection.RemoteIpAddress?.ToString());
                    diagnosticContext.Set("UserAgent", httpContext.Request.Headers["User-Agent"].ToString());
                    
                    if (httpContext.User.Identity?.IsAuthenticated == true)
                    {
                        diagnosticContext.Set("UserName", httpContext.User.Identity.Name);
                        diagnosticContext.Set("UserId", httpContext.User.FindFirst("sub")?.Value 
                            ?? httpContext.User.FindFirst("id")?.Value);
                    }
                };

                // Customize log message template
                opts.MessageTemplate = "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms";
            });
        }

        return app;
    }
}
