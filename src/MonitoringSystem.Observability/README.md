# MonitoringSystem.Observability Library

This is the core observability library for .NET 8.0+ applications.

## Package Contents

### Configuration
- `ObservabilityOptions` - Main configuration class
- `LoggingOptions` - Logging-specific configuration
- `FileLoggingOptions` - File sink configuration
- `SensitiveDataOptions` - Sensitive data masking configuration

### Enrichers
- `CorrelationIdEnricher` - Adds correlation IDs to log events
- `SensitiveDataMaskingEnricher` - Masks sensitive data in logs
- `ICorrelationIdProvider` - Interface for correlation ID management
- `ISensitiveDataMasker` - Interface for custom masking strategies

### Middleware
- `CorrelationIdMiddleware` - Manages correlation IDs for HTTP requests
- `GlobalErrorHandlingMiddleware` - Catches unhandled exceptions and returns a standardized error response.

### Extensions
- `ObservabilityServiceCollectionExtensions` - DI container configuration
- `ObservabilityApplicationBuilderExtensions` - Middleware pipeline configuration
- `GlobalErrorHandlingExtensions` - Provides the `UseGlobalErrorHandling()` extension method.

## Usage

See the main README.md in the repository root for complete usage instructions.

## Dependencies

- Serilog.AspNetCore (8.0.3)
- Serilog.Enrichers.Environment (3.0.1)
- Serilog.Enrichers.Thread (4.0.0)
- Microsoft.AspNetCore.Http.Abstractions (2.2.0)

## Target Framework

- .NET 8.0

## Extensibility Points

### Custom Enrichers

Implement `Serilog.Core.ILogEventEnricher`:

```csharp
public class MyCustomEnricher : ILogEventEnricher
{
    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
    {
        var property = propertyFactory.CreateProperty("CustomProperty", "CustomValue");
        logEvent.AddPropertyIfAbsent(property);
    }
}

// Add to configuration
loggerConfiguration.Enrich.With<MyCustomEnricher>();
```

### Custom Sensitive Data Masker

Implement `ISensitiveDataMasker`:

```csharp
public class MyCustomMasker : ISensitiveDataMasker
{
    public string Mask(string value)
    {
        // Custom masking logic
        return "***";
    }
}

// Register in DI
services.AddSingleton<ISensitiveDataMasker, MyCustomMasker>();
```

### Custom Correlation ID Provider

Implement `ICorrelationIdProvider`:

```csharp
public class MyCorrelationIdProvider : ICorrelationIdProvider
{
    public string? GetCorrelationId()
    {
        // Custom logic to retrieve correlation ID
    }

    public void SetCorrelationId(string correlationId)
    {
        // Custom logic to store correlation ID
    }
}

// Register in DI
services.AddSingleton<ICorrelationIdProvider, MyCorrelationIdProvider>();

## Global Error Handling

This library includes a middleware for global exception handling. It catches any unhandled exceptions, logs them using the configured structured logger, and returns a standardized `500 Internal Server Error` JSON response.

### Enabling Global Error Handling

To enable it, register the middleware in your application's request pipeline. It should be placed early in the pipeline to ensure it can catch exceptions from subsequent middlewares.

**Example in `Program.cs`:**

```csharp
var app = builder.Build();

// It is recommended to place the error handler early in the pipeline.
app.UseGlobalErrorHandling();

// ... other middlewares
```

When an exception is caught, the middleware will log the full exception details and return a response similar to this:

```json
{
  "StatusCode": 500,
  "Message": "An internal server error has occurred.",
  "Detailed": "The actual exception message."
}
```
