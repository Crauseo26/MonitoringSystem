using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;
using Serilog.Formatting.Compact;
using MonitoringSystem.Observability.Configuration;
using MonitoringSystem.Observability.Enrichers;

namespace MonitoringSystem.Observability.Extensions;

/// <summary>
/// Extension methods for configuring observability services
/// </summary>
public static class ObservabilityServiceCollectionExtensions
{
    /// <summary>
    /// Adds observability services with default configuration from appsettings.json
    /// </summary>
    public static IServiceCollection AddObservability(
        this IServiceCollection services,
        IConfiguration configuration,
        string configurationSection = "Observability")
    {
        var options = new ObservabilityOptions();
        configuration.GetSection(configurationSection).Bind(options);

        return services.AddObservability(options);
    }

    /// <summary>
    /// Adds observability services with custom configuration
    /// </summary>
    public static IServiceCollection AddObservability(
        this IServiceCollection services,
        Action<ObservabilityOptions> configureOptions)
    {
        var options = new ObservabilityOptions();
        configureOptions(options);

        return services.AddObservability(options);
    }

    /// <summary>
    /// Adds observability services with provided options
    /// </summary>
    public static IServiceCollection AddObservability(
        this IServiceCollection services,
        ObservabilityOptions options)
    {
        // Register correlation ID provider
        services.AddSingleton<ICorrelationIdProvider, AsyncLocalCorrelationIdProvider>();

        // Register sensitive data masker
        services.AddSingleton<ISensitiveDataMasker>(sp => 
            new DefaultSensitiveDataMasker(options.SensitiveData));

        // Store options for later use
        services.AddSingleton(options);

        return services;
    }

    /// <summary>
    /// Configures Serilog with the observability options
    /// </summary>
    public static IHostBuilder UseObservability(
        this IHostBuilder hostBuilder,
        IConfiguration? configuration = null,
        string configurationSection = "Observability")
    {
        return hostBuilder.UseSerilog((context, services, loggerConfiguration) =>
        {
            var options = new ObservabilityOptions();
            
            if (configuration != null)
            {
                configuration.GetSection(configurationSection).Bind(options);
            }
            else
            {
                context.Configuration.GetSection(configurationSection).Bind(options);
            }

            ConfigureSerilog(loggerConfiguration, options, services);
        });
    }

    /// <summary>
    /// Configures Serilog with custom options
    /// </summary>
    public static IHostBuilder UseObservability(
        this IHostBuilder hostBuilder,
        Action<ObservabilityOptions> configureOptions)
    {
        return hostBuilder.UseSerilog((context, services, loggerConfiguration) =>
        {
            var options = new ObservabilityOptions();
            configureOptions(options);

            ConfigureSerilog(loggerConfiguration, options, services);
        });
    }

    private static void ConfigureSerilog(
        LoggerConfiguration loggerConfiguration,
        ObservabilityOptions options,
        IServiceProvider services)
    {
        // Set minimum level
        var minimumLevel = ParseLogLevel(options.Logging.MinimumLevel);
        loggerConfiguration.MinimumLevel.Is(minimumLevel);

        // Apply level overrides
        foreach (var (source, level) in options.Logging.MinimumLevelOverrides)
        {
            var logLevel = ParseLogLevel(level);
            loggerConfiguration.MinimumLevel.Override(source, logLevel);
        }

        // Enrich with standard properties
        loggerConfiguration
            .Enrich.FromLogContext()
            .Enrich.WithMachineName()
            .Enrich.WithThreadId()
            .Enrich.WithProperty("Application", options.ApplicationName)
            .Enrich.WithProperty("Environment", options.Environment);

        // Add correlation ID enricher
        var correlationIdProvider = services.GetService<ICorrelationIdProvider>();
        if (correlationIdProvider != null)
        {
            loggerConfiguration.Enrich.With(new CorrelationIdEnricher(correlationIdProvider));
        }

        // Add sensitive data masking enricher
        if (options.SensitiveData.EnableMasking)
        {
            var masker = services.GetService<ISensitiveDataMasker>();
            loggerConfiguration.Enrich.With(new SensitiveDataMaskingEnricher(options.SensitiveData, masker));
        }

        // Configure console sink
        if (options.Logging.EnableConsole)
        {
            if (options.Logging.UseJsonFormat)
            {
                loggerConfiguration.WriteTo.Console(new CompactJsonFormatter());
            }
            else
            {
                loggerConfiguration.WriteTo.Console(
                    outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}");
            }
        }

        // Configure file sink
        if (options.Logging.EnableFile)
        {
            var fileOptions = options.Logging.File;
            var rollingInterval = ParseRollingInterval(fileOptions.RollingInterval);

            if (options.Logging.UseJsonFormat)
            {
                loggerConfiguration.WriteTo.File(
                    new CompactJsonFormatter(),
                    path: Path.Combine(fileOptions.Path, fileOptions.FileNamePattern),
                    rollingInterval: rollingInterval,
                    retainedFileCountLimit: fileOptions.RetainedFileCountLimit,
                    fileSizeLimitBytes: fileOptions.FileSizeLimitBytes,
                    shared: fileOptions.Shared);
            }
            else
            {
                loggerConfiguration.WriteTo.File(
                    path: Path.Combine(fileOptions.Path, fileOptions.FileNamePattern),
                    rollingInterval: rollingInterval,
                    retainedFileCountLimit: fileOptions.RetainedFileCountLimit,
                    fileSizeLimitBytes: fileOptions.FileSizeLimitBytes,
                    shared: fileOptions.Shared,
                    outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz}] [{Level:u3}] [{SourceContext}] {Message:lj} {Properties:j}{NewLine}{Exception}");
            }
        }
    }

    private static LogEventLevel ParseLogLevel(string level)
    {
        return level.ToLowerInvariant() switch
        {
            "verbose" => LogEventLevel.Verbose,
            "debug" => LogEventLevel.Debug,
            "information" => LogEventLevel.Information,
            "warning" => LogEventLevel.Warning,
            "error" => LogEventLevel.Error,
            "fatal" => LogEventLevel.Fatal,
            _ => LogEventLevel.Information
        };
    }

    private static RollingInterval ParseRollingInterval(string interval)
    {
        return interval.ToLowerInvariant() switch
        {
            "infinite" => RollingInterval.Infinite,
            "year" => RollingInterval.Year,
            "month" => RollingInterval.Month,
            "day" => RollingInterval.Day,
            "hour" => RollingInterval.Hour,
            "minute" => RollingInterval.Minute,
            _ => RollingInterval.Day
        };
    }
}
