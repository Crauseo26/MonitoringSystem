# MonitoringSystem.Observability

A reusable .NET 8.0+ library for implementing structured logging and observability in ASP.NET Core applications.

## Features

- ✅ **Structured JSON Logging** - JSON formatted logs for easy parsing and analysis
- ✅ **Correlation ID Tracking** - Automatic request correlation across distributed systems
- ✅ **Sensitive Data Masking** - Configurable masking of sensitive information (passwords, tokens, etc.)
- ✅ **Request Logging Middleware** - Automatic HTTP request/response logging
- ✅ **Flexible Configuration** - Configure via appsettings.json or code
- ✅ **Multiple Sinks** - Console and file logging with rotation support
- ✅ **Log Enrichment** - Automatic enrichment with machine name, thread ID, environment, etc.
- ✅ **Extensible** - Easy to extend with custom enrichers and masking strategies
- ✅ **Production Ready** - Built on Serilog, a mature and battle-tested logging framework
- ✅ **Future-Proof** - Designed to extend to metrics and distributed tracing

## Installation

### From Source

1. Clone the repository
2. Build the solution:
   ```bash
   dotnet build
   ```
3. Reference the project in your application:
   ```xml
   <ProjectReference Include="path\to\MonitoringSystem.Observability\MonitoringSystem.Observability.csproj" />
   ```

### As NuGet Package (Future)

```bash
dotnet add package MonitoringSystem.Observability
```

## Quick Start

### 1. Configure in Program.cs

```csharp
using MonitoringSystem.Observability.Extensions;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Configure Observability (Serilog)
builder.Host.UseObservability(builder.Configuration);

// Add Observability services
builder.Services.AddObservability(builder.Configuration);

// ... other services

var app = builder.Build();

// Use Observability middleware (Correlation ID and Request Logging)
app.UseObservability();

// ... other middleware

app.Run();

// Ensure Serilog flushes on shutdown
Log.CloseAndFlush();
```

### 2. Configure in appsettings.json

```json
{
  "Observability": {
    "ApplicationName": "MyApi",
    "Environment": "Production",
    "Logging": {
      "MinimumLevel": "Information",
      "EnableConsole": true,
      "EnableFile": true,
      "UseJsonFormat": true,
      "EnableRequestLogging": true,
      "File": {
        "Path": "./logs",
        "FileNamePattern": "log-.txt",
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
        "password",
        "token",
        "apikey",
        "secret"
      ]
    }
  }
}
```

### 3. Use in Your Controllers

```csharp
[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly ILogger<UsersController> _logger;

    public UsersController(ILogger<UsersController> logger)
    {
        _logger = logger;
    }

    [HttpGet("{id}")]
    public IActionResult GetUser(int id)
    {
        _logger.LogInformation("Fetching user with ID: {UserId}", id);
        
        // Your logic here
        
        return Ok(user);
    }
}
```

## Configuration Options

### ObservabilityOptions

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `ApplicationName` | string | "Application" | Name of your application |
| `Environment` | string | "Development" | Environment name (Development, Staging, Production) |
| `Logging` | LoggingOptions | - | Logging configuration |
| `SensitiveData` | SensitiveDataOptions | - | Sensitive data masking configuration |

### LoggingOptions

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `MinimumLevel` | string | "Information" | Minimum log level (Verbose, Debug, Information, Warning, Error, Fatal) |
| `EnableConsole` | bool | true | Enable console logging |
| `EnableFile` | bool | true | Enable file logging |
| `UseJsonFormat` | bool | true | Use JSON format for logs |
| `EnableRequestLogging` | bool | true | Enable HTTP request logging middleware |
| `File` | FileLoggingOptions | - | File logging configuration |
| `MinimumLevelOverrides` | Dictionary | - | Override minimum levels for specific namespaces |

### FileLoggingOptions

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `Path` | string | "./logs" | Directory for log files |
| `FileNamePattern` | string | "log-.txt" | Log file name pattern |
| `RollingInterval` | string | "Day" | Rolling interval (Infinite, Year, Month, Day, Hour, Minute) |
| `RetainedFileCountLimit` | int? | 31 | Number of log files to retain |
| `FileSizeLimitBytes` | long? | 104857600 | Maximum file size (100 MB default) |
| `Shared` | bool | true | Enable shared file access |

### SensitiveDataOptions

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `EnableMasking` | bool | true | Enable automatic masking |
| `PropertyNamesToMask` | List<string> | ["password", "token", ...] | Property names to mask |
| `MaskCharacter` | string | "*" | Character to use for masking |
| `ShowLastCharacters` | int | 0 | Number of characters to show at the end |

## Advanced Usage

### Custom Configuration via Code

```csharp
builder.Services.AddObservability(options =>
{
    options.ApplicationName = "MyCustomApi";
    options.Environment = "Staging";
    options.Logging.MinimumLevel = "Debug";
    options.Logging.UseJsonFormat = false;
});
```

### Custom Sensitive Data Masker

Implement the `ISensitiveDataMasker` interface:

```csharp
public class CustomMasker : ISensitiveDataMasker
{
    public string Mask(string value)
    {
        // Your custom masking logic
        return "***REDACTED***";
    }
}

// Register in DI
builder.Services.AddSingleton<ISensitiveDataMasker, CustomMasker>();
```

### Using Log Scopes

```csharp
using (_logger.BeginScope(new Dictionary<string, object>
{
    ["UserId"] = userId,
    ["Operation"] = "UpdateProfile"
}))
{
    _logger.LogInformation("Starting profile update");
    // All logs in this scope will include UserId and Operation
}
```

### Correlation ID

The library automatically manages correlation IDs:

- Reads `X-Correlation-ID` header from incoming requests
- Generates a new GUID if not present
- Adds correlation ID to all logs
- Returns correlation ID in response headers

You can also manually access the correlation ID:

```csharp
public class MyService
{
    private readonly ICorrelationIdProvider _correlationIdProvider;

    public MyService(ICorrelationIdProvider correlationIdProvider)
    {
        _correlationIdProvider = correlationIdProvider;
    }

    public void DoSomething()
    {
        var correlationId = _correlationIdProvider.GetCorrelationId();
        // Use correlation ID
    }
}
```

## Sample Application

The `samples/SampleApi` project demonstrates all features:

1. Navigate to the sample API:
   ```bash
   cd samples/SampleApi
   ```

2. Run the application:
   ```bash
   dotnet run
   ```

3. Test the endpoints:
   ```bash
   # Get all users
   curl https://localhost:7XXX/api/users

   # Get specific user
   curl https://localhost:7XXX/api/users/1

   # Create user (demonstrates sensitive data masking)
   curl -X POST https://localhost:7XXX/api/users \
     -H "Content-Type: application/json" \
     -d '{"name":"John","email":"john@test.com","password":"secret123"}'

   # Simulate error
   curl https://localhost:7XXX/api/users/error
   ```

4. Check the logs in `./logs` directory

## Log Output Examples

### JSON Format (Production)

```json
{
  "@t": "2025-10-13T21:30:00.1234567Z",
  "@l": "Information",
  "@mt": "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms",
  "RequestMethod": "GET",
  "RequestPath": "/api/users/1",
  "StatusCode": 200,
  "Elapsed": 45.6789,
  "CorrelationId": "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
  "Application": "SampleApi",
  "Environment": "Production",
  "MachineName": "WEB-SERVER-01",
  "ThreadId": 12
}
```

### Text Format (Development)

```
[21:30:00 INF] HTTP GET /api/users/1 responded 200 in 45.6789 ms
  CorrelationId="a1b2c3d4-e5f6-7890-abcd-ef1234567890"
  Application="SampleApi"
  Environment="Development"
```

## Architecture

```
MonitoringSystem.Observability/
├── Configuration/
│   └── ObservabilityOptions.cs       # Configuration models
├── Enrichers/
│   ├── CorrelationIdEnricher.cs      # Adds correlation ID to logs
│   └── SensitiveDataMaskingEnricher.cs # Masks sensitive data
├── Middleware/
│   └── CorrelationIdMiddleware.cs    # Manages correlation IDs
└── Extensions/
    ├── ObservabilityServiceCollectionExtensions.cs  # DI setup
    └── ObservabilityApplicationBuilderExtensions.cs # Middleware setup
```

## Extending to Metrics and Traces

This library is designed to be extended with OpenTelemetry for metrics and distributed tracing:

```csharp
// Future implementation
builder.Services.AddObservability(options =>
{
    options.EnableMetrics = true;
    options.EnableTracing = true;
    options.Metrics.ExportToPrometheus = true;
    options.Tracing.ExportToJaeger = true;
});
```

## Best Practices

1. **Use Structured Logging**: Always use structured logging with named parameters
   ```csharp
   // Good
   _logger.LogInformation("User {UserId} logged in", userId);
   
   // Bad
   _logger.LogInformation($"User {userId} logged in");
   ```

2. **Use Appropriate Log Levels**:
   - `Verbose`: Very detailed diagnostic information
   - `Debug`: Debugging information
   - `Information`: General informational messages
   - `Warning`: Warning messages for potentially harmful situations
   - `Error`: Error messages for failures
   - `Fatal`: Critical failures that require immediate attention

3. **Use Log Scopes**: Group related log entries with scopes

4. **Configure by Environment**: Use different settings for Development vs Production

5. **Monitor Log File Size**: Configure appropriate retention and size limits

## Contributing

Contributions are welcome! Please follow these guidelines:

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Add tests if applicable
5. Submit a pull request

## License

This project is licensed under the MIT License - see the LICENSE file for details.

## Support

For issues, questions, or suggestions, please open an issue on GitHub.

## Roadmap

- [ ] NuGet package publication
- [ ] OpenTelemetry integration for metrics
- [ ] OpenTelemetry integration for distributed tracing
- [ ] Additional sinks (Elasticsearch, Seq, Application Insights)
- [ ] Performance metrics collection
- [ ] Health check endpoints
- [ ] Log sampling for high-volume scenarios
- [ ] Async logging optimization
