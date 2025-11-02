# Quick Start Guide

Get up and running with MonitoringSystem.Observability in 5 minutes.

## Step 1: Add the Package

Add a reference to the MonitoringSystem.Observability project:

```xml
<ItemGroup>
  <ProjectReference Include="path\to\MonitoringSystem.Observability\MonitoringSystem.Observability.csproj" />
</ItemGroup>
```

## Step 2: Configure Program.cs

Update your `Program.cs`:

```csharp
using MonitoringSystem.Observability.Extensions;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add Observability
builder.Host.UseObservability(builder.Configuration);
builder.Services.AddObservability(builder.Configuration);

// Your other services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Add Observability middleware
app.UseObservability();

app.MapControllers();

app.Run();

// Ensure logs are flushed
Log.CloseAndFlush();
```

## Step 3: Add Configuration

Create or update `appsettings.json`:

```json
{
  "Observability": {
    "ApplicationName": "MyApi",
    "Environment": "Development",
    "Logging": {
      "MinimumLevel": "Information",
      "EnableConsole": true,
      "EnableFile": true,
      "UseJsonFormat": true
    }
  }
}
```

## Step 4: Use in Your Controllers

```csharp
using Microsoft.AspNetCore.Mvc;

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
        
        var user = new { Id = id, Name = $"User {id}" };
        
        _logger.LogInformation("User retrieved: {@User}", user);
        
        return Ok(user);
    }
}
```

## Step 5: Run and Test

1. **Run your application:**
   ```bash
   dotnet run
   ```

2. **Make a request:**
   ```bash
   curl https://localhost:7XXX/api/users/1
   ```

3. **Check the logs:**
   - Console: Real-time output
   - Files: `./logs/log-YYYYMMDD.txt`

## What You Get

✅ **Structured JSON logs** in production  
✅ **Correlation IDs** for request tracking  
✅ **Automatic request logging** with timing  
✅ **Sensitive data masking** (passwords, tokens, etc.)  
✅ **File rotation** and retention  
✅ **Configurable log levels**  

## Example Log Output

### Console (Development)
```
[21:30:00 INF] HTTP GET /api/users/1 responded 200 in 45.67 ms
[21:30:00 INF] Fetching user with ID: 1
[21:30:00 INF] User retrieved: {"Id": 1, "Name": "User 1"}
```

### File (JSON)
```json
{
  "@t": "2025-10-13T21:30:00.1234567Z",
  "@l": "Information",
  "@mt": "Fetching user with ID: {UserId}",
  "UserId": 1,
  "CorrelationId": "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
  "Application": "MyApi",
  "Environment": "Development"
}
```

## Next Steps

- 📖 Read the [full documentation](README.md)
- 🔧 Explore [advanced configuration](USAGE_GUIDE.md)
- 💡 Check the [sample application](samples/SampleApi)
- 🎯 Learn [best practices](USAGE_GUIDE.md#logging-best-practices)

## Common Configurations

### Production Settings

```json
{
  "Observability": {
    "ApplicationName": "MyApi",
    "Environment": "Production",
    "Logging": {
      "MinimumLevel": "Information",
      "UseJsonFormat": true,
      "File": {
        "Path": "./logs",
        "RollingInterval": "Day",
        "RetainedFileCountLimit": 31
      },
      "MinimumLevelOverrides": {
        "Microsoft": "Warning",
        "System": "Warning"
      }
    }
  }
}
```

### Development Settings

```json
{
  "Observability": {
    "ApplicationName": "MyApi",
    "Environment": "Development",
    "Logging": {
      "MinimumLevel": "Debug",
      "UseJsonFormat": false,
      "EnableConsole": true
    }
  }
}
```

## Troubleshooting

### Logs not appearing?
- Check that `UseObservability()` is called in Program.cs
- Verify log level configuration
- Ensure `Log.CloseAndFlush()` is called on shutdown

### Need help?
- Check the [Usage Guide](USAGE_GUIDE.md)
- Review the [Sample API](samples/SampleApi)
- Open an issue on GitHub

---

**That's it! You now have enterprise-grade observability in your application.** 🚀
