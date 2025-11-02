# MonitoringSystem.Observability - Usage Guide

Complete guide for implementing observability in your .NET applications.

## Table of Contents

1. [Getting Started](#getting-started)
2. [Basic Configuration](#basic-configuration)
3. [Advanced Configuration](#advanced-configuration)
4. [Logging Best Practices](#logging-best-practices)
5. [Correlation ID Management](#correlation-id-management)
6. [Sensitive Data Masking](#sensitive-data-masking)
7. [Performance Considerations](#performance-considerations)
8. [Troubleshooting](#troubleshooting)
9. [Migration Guide](#migration-guide)
10. [Future Extensions](#future-extensions)

---

## Getting Started

### Prerequisites

- .NET 8.0 SDK or later
- ASP.NET Core application

### Installation Steps

1. **Add Project Reference**

   ```xml
   <ItemGroup>
     <ProjectReference Include="path\to\MonitoringSystem.Observability\MonitoringSystem.Observability.csproj" />
   </ItemGroup>
   ```

2. **Configure Program.cs**

   ```csharp
   using MonitoringSystem.Observability.Extensions;
   using Serilog;

   var builder = WebApplication.CreateBuilder(args);

   // Configure Observability
   builder.Host.UseObservability(builder.Configuration);
   builder.Services.AddObservability(builder.Configuration);

   // ... other services

   var app = builder.Build();

   // Add Observability middleware
   app.UseObservability();

   // ... other middleware

   app.Run();
   Log.CloseAndFlush();
   ```

3. **Add Configuration**

   Create or update `appsettings.json` (see [Basic Configuration](#basic-configuration))

---

## Basic Configuration

### Minimal Configuration

```json
{
  "Observability": {
    "ApplicationName": "MyApi",
    "Environment": "Development"
  }
}
```

This uses all default values:
- Console logging: Enabled
- File logging: Enabled (./logs directory)
- JSON format: Enabled
- Log level: Information
- Sensitive data masking: Enabled

### Recommended Development Configuration

```json
{
  "Observability": {
    "ApplicationName": "MyApi",
    "Environment": "Development",
    "Logging": {
      "MinimumLevel": "Debug",
      "UseJsonFormat": false,
      "EnableConsole": true,
      "EnableFile": true,
      "EnableRequestLogging": true
    }
  }
}
```

### Recommended Production Configuration

```json
{
  "Observability": {
    "ApplicationName": "MyApi",
    "Environment": "Production",
    "Logging": {
      "MinimumLevel": "Information",
      "UseJsonFormat": true,
      "EnableConsole": true,
      "EnableFile": true,
      "EnableRequestLogging": true,
      "File": {
        "Path": "./logs",
        "RollingInterval": "Day",
        "RetainedFileCountLimit": 31,
        "FileSizeLimitBytes": 104857600
      },
      "MinimumLevelOverrides": {
        "Microsoft": "Warning",
        "System": "Warning"
      }
    },
    "SensitiveData": {
      "EnableMasking": true,
      "PropertyNamesToMask": [
        "password", "token", "apikey", "secret"
      ]
    }
  }
}
```

---

## Advanced Configuration

### Configuration via Code

```csharp
builder.Services.AddObservability(options =>
{
    options.ApplicationName = "MyApi";
    options.Environment = builder.Environment.EnvironmentName;
    
    options.Logging.MinimumLevel = "Information";
    options.Logging.UseJsonFormat = true;
    
    options.Logging.File.Path = "/var/log/myapi";
    options.Logging.File.RollingInterval = "Hour";
    
    options.SensitiveData.PropertyNamesToMask.Add("customSecret");
});
```

### Environment-Specific Configuration

Create multiple configuration files:
- `appsettings.json` - Base configuration
- `appsettings.Development.json` - Development overrides
- `appsettings.Staging.json` - Staging overrides
- `appsettings.Production.json` - Production overrides

### Custom Configuration Section

```csharp
// Use a different configuration section name
builder.Services.AddObservability(
    builder.Configuration, 
    configurationSection: "CustomLogging"
);
```

### Disable Specific Features

```json
{
  "Observability": {
    "Logging": {
      "EnableConsole": false,
      "EnableFile": true,
      "EnableRequestLogging": false
    },
    "SensitiveData": {
      "EnableMasking": false
    }
  }
}
```

---

## Logging Best Practices

### 1. Use Structured Logging

**✅ Good:**
```csharp
_logger.LogInformation("User {UserId} logged in from {IpAddress}", userId, ipAddress);
```

**❌ Bad:**
```csharp
_logger.LogInformation($"User {userId} logged in from {ipAddress}");
```

### 2. Use Appropriate Log Levels

```csharp
// Verbose - Very detailed, typically only for debugging
_logger.LogTrace("Entering method {MethodName}", nameof(GetUser));

// Debug - Debugging information
_logger.LogDebug("Cache miss for key {CacheKey}", key);

// Information - General flow of the application
_logger.LogInformation("User {UserId} created successfully", userId);

// Warning - Unexpected but handled situations
_logger.LogWarning("Rate limit exceeded for user {UserId}", userId);

// Error - Errors and exceptions
_logger.LogError(ex, "Failed to process order {OrderId}", orderId);

// Fatal - Critical failures
_logger.LogCritical("Database connection lost");
```

### 3. Use Destructuring for Complex Objects

```csharp
// @ symbol destructures the object
_logger.LogInformation("Created user: {@User}", user);

// Without @, only ToString() is called
_logger.LogInformation("Created user: {User}", user);
```

### 4. Use Log Scopes for Context

```csharp
using (_logger.BeginScope(new Dictionary<string, object>
{
    ["UserId"] = userId,
    ["Operation"] = "UpdateProfile",
    ["RequestId"] = requestId
}))
{
    _logger.LogInformation("Starting profile update");
    // All logs in this scope include the context
    await UpdateProfileAsync();
    _logger.LogInformation("Profile update completed");
}
```

### 5. Don't Log Sensitive Data Directly

```csharp
// ❌ Bad - logs password in plain text
_logger.LogDebug("Login attempt with password: {Password}", password);

// ✅ Good - let the masking enricher handle it
_logger.LogDebug("Login attempt: {@LoginRequest}", loginRequest);

// ✅ Better - don't log sensitive data at all
_logger.LogInformation("Login attempt for user {Username}", username);
```

### 6. Log Exceptions Properly

```csharp
try
{
    await ProcessOrderAsync(orderId);
}
catch (Exception ex)
{
    // ✅ Good - includes exception details
    _logger.LogError(ex, "Failed to process order {OrderId}", orderId);
    throw;
}
```

### 7. Use Meaningful Property Names

```csharp
// ✅ Good - clear property names
_logger.LogInformation("Order {OrderId} processed in {ElapsedMs}ms", orderId, elapsed);

// ❌ Bad - unclear property names
_logger.LogInformation("Order {Id} processed in {Time}ms", orderId, elapsed);
```

---

## Correlation ID Management

### Automatic Correlation ID

The middleware automatically:
1. Reads `X-Correlation-ID` from request headers
2. Generates a new GUID if not present
3. Adds correlation ID to all logs
4. Returns correlation ID in response headers

### Manual Correlation ID Access

```csharp
public class MyService
{
    private readonly ICorrelationIdProvider _correlationIdProvider;
    private readonly ILogger<MyService> _logger;

    public MyService(
        ICorrelationIdProvider correlationIdProvider,
        ILogger<MyService> logger)
    {
        _correlationIdProvider = correlationIdProvider;
        _logger = logger;
    }

    public async Task ProcessAsync()
    {
        var correlationId = _correlationIdProvider.GetCorrelationId();
        
        // Use correlation ID for external API calls
        await CallExternalApiAsync(correlationId);
        
        _logger.LogInformation("Processing completed");
        // Log automatically includes correlation ID
    }
}
```

### Custom Correlation ID Provider

```csharp
public class CustomCorrelationIdProvider : ICorrelationIdProvider
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CustomCorrelationIdProvider(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string? GetCorrelationId()
    {
        return _httpContextAccessor.HttpContext?.TraceIdentifier;
    }

    public void SetCorrelationId(string correlationId)
    {
        // Custom storage logic
    }
}

// Register in DI
builder.Services.AddSingleton<ICorrelationIdProvider, CustomCorrelationIdProvider>();
```

---

## Sensitive Data Masking

### Default Masked Properties

By default, these property names are masked (case-insensitive):
- password, pwd
- secret
- token
- apikey, api_key
- authorization, auth
- creditcard, credit_card
- ssn, social_security

### Add Custom Properties to Mask

```json
{
  "Observability": {
    "SensitiveData": {
      "PropertyNamesToMask": [
        "password",
        "token",
        "customSecret",
        "privateKey",
        "bankAccount"
      ]
    }
  }
}
```

### Custom Masking Strategy

```csharp
public class CustomMasker : ISensitiveDataMasker
{
    public string Mask(string value)
    {
        if (string.IsNullOrEmpty(value))
            return value;

        // Show first 2 and last 2 characters
        if (value.Length <= 4)
            return "****";

        var first = value.Substring(0, 2);
        var last = value.Substring(value.Length - 2);
        var masked = new string('*', value.Length - 4);

        return $"{first}{masked}{last}";
    }
}

// Register in DI
builder.Services.AddSingleton<ISensitiveDataMasker, CustomMasker>();
```

### Partial Masking

```json
{
  "Observability": {
    "SensitiveData": {
      "ShowLastCharacters": 4
    }
  }
}
```

This shows the last 4 characters: `password123` → `*********123`

---

## Performance Considerations

### 1. Use Appropriate Log Levels

In production, set minimum level to `Information` or higher:

```json
{
  "Observability": {
    "Logging": {
      "MinimumLevel": "Information"
    }
  }
}
```

### 2. Override Noisy Namespaces

```json
{
  "Observability": {
    "Logging": {
      "MinimumLevelOverrides": {
        "Microsoft": "Warning",
        "Microsoft.EntityFrameworkCore": "Warning",
        "System.Net.Http.HttpClient": "Warning"
      }
    }
  }
}
```

### 3. Configure File Rotation

```json
{
  "Observability": {
    "Logging": {
      "File": {
        "RollingInterval": "Day",
        "RetainedFileCountLimit": 31,
        "FileSizeLimitBytes": 104857600
      }
    }
  }
}
```

### 4. Disable Console in Production (Optional)

```json
{
  "Observability": {
    "Logging": {
      "EnableConsole": false,
      "EnableFile": true
    }
  }
}
```

### 5. Use Shared File Access

```json
{
  "Observability": {
    "Logging": {
      "File": {
        "Shared": true
      }
    }
  }
}
```

---

## Troubleshooting

### Logs Not Appearing

**Problem:** No logs are being written

**Solutions:**
1. Check that `UseObservability()` is called in Program.cs
2. Verify log level configuration
3. Check file permissions for log directory
4. Ensure `Log.CloseAndFlush()` is called on shutdown

### Correlation ID Not in Logs

**Problem:** Correlation ID is missing from log entries

**Solutions:**
1. Verify `UseObservability()` is called before other middleware
2. Check that `ICorrelationIdProvider` is registered in DI
3. Ensure the enricher is configured properly

### Sensitive Data Not Masked

**Problem:** Passwords or tokens appear in logs

**Solutions:**
1. Check that `EnableMasking` is `true`
2. Verify property names match configuration
3. Ensure the masking enricher is registered
4. Check that you're using structured logging (not string interpolation)

### File Permission Errors

**Problem:** Cannot write to log files

**Solutions:**
1. Check directory permissions
2. Run application with appropriate user permissions
3. Change log path to a writable location
4. Use `Shared: true` for multi-process scenarios

### Large Log Files

**Problem:** Log files growing too large

**Solutions:**
1. Reduce log level in production
2. Configure file size limits
3. Set appropriate retention policy
4. Override noisy namespaces
5. Consider using log aggregation services

---

## Migration Guide

### From Microsoft.Extensions.Logging

```csharp
// Before
builder.Services.AddLogging();

// After
builder.Host.UseObservability(builder.Configuration);
builder.Services.AddObservability(builder.Configuration);
```

Your existing `ILogger<T>` usage remains unchanged!

### From Serilog Direct Configuration

```csharp
// Before
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("logs/log.txt")
    .CreateLogger();

// After
builder.Host.UseObservability(builder.Configuration);
builder.Services.AddObservability(builder.Configuration);
```

Configure via `appsettings.json` instead.

---

## Future Extensions

### Metrics (Planned)

```csharp
builder.Services.AddObservability(options =>
{
    options.EnableMetrics = true;
    options.Metrics.ExportToPrometheus = true;
});
```

### Distributed Tracing (Planned)

```csharp
builder.Services.AddObservability(options =>
{
    options.EnableTracing = true;
    options.Tracing.ExportToJaeger = true;
});
```

### Additional Sinks (Planned)

- Elasticsearch
- Seq
- Application Insights
- Datadog
- New Relic

---

## Additional Resources

- [Serilog Documentation](https://serilog.net/)
- [OpenTelemetry .NET](https://opentelemetry.io/docs/instrumentation/net/)
- [Structured Logging Best Practices](https://github.com/serilog/serilog/wiki/Structured-Data)

---

## Support

For issues or questions:
1. Check this guide
2. Review the sample application
3. Open an issue on GitHub
