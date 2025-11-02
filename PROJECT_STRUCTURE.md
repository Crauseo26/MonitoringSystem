# Project Structure

This document describes the organization and structure of the MonitoringSystem.Observability project.

## Directory Structure

```
MonitoringSystem/
├── src/
│   └── MonitoringSystem.Observability/          # Core observability library
│       ├── Configuration/
│       │   └── ObservabilityOptions.cs          # Configuration models
│       ├── Enrichers/
│       │   ├── CorrelationIdEnricher.cs         # Correlation ID enrichment
│       │   └── SensitiveDataMaskingEnricher.cs  # Sensitive data masking
│       ├── Middleware/
│       │   └── CorrelationIdMiddleware.cs       # Correlation ID management
│       ├── Extensions/
│       │   ├── ObservabilityServiceCollectionExtensions.cs  # DI setup
│       │   └── ObservabilityApplicationBuilderExtensions.cs # Middleware setup
│       ├── MonitoringSystem.Observability.csproj
│       └── README.md
│
├── samples/
│   └── SampleApi/                               # Sample API demonstrating usage
│       ├── Controllers/
│       │   ├── UsersController.cs               # User management endpoints
│       │   └── ProductsController.cs            # Product management endpoints
│       ├── appsettings.json                     # Production configuration
│       ├── appsettings.Development.json         # Development configuration
│       ├── Program.cs                           # Application entry point
│       ├── SampleApi.csproj
│       └── README.md
│
├── tests/                                       # Unit tests (future)
│   └── MonitoringSystem.Observability.Tests/
│
├── docs/                                        # Additional documentation (future)
│
├── .gitignore                                   # Git ignore rules
├── MonitoringSystem.sln                         # Solution file
├── README.md                                    # Main documentation
├── QUICKSTART.md                                # Quick start guide
├── USAGE_GUIDE.md                               # Comprehensive usage guide
├── CONTRIBUTING.md                              # Contribution guidelines
├── CHANGELOG.md                                 # Version history
├── LICENSE                                      # MIT License
└── PROJECT_STRUCTURE.md                         # This file
```

## Core Library Components

### Configuration (`Configuration/`)

**ObservabilityOptions.cs**
- Main configuration class
- Contains nested configuration for logging, file settings, and sensitive data
- Designed for binding from appsettings.json

### Enrichers (`Enrichers/`)

**CorrelationIdEnricher.cs**
- Adds correlation IDs to log events
- Integrates with `ICorrelationIdProvider`
- Enables request tracking across distributed systems

**SensitiveDataMaskingEnricher.cs**
- Masks sensitive data in log events
- Configurable property names to mask
- Extensible through `ISensitiveDataMasker` interface

### Middleware (`Middleware/`)

**CorrelationIdMiddleware.cs**
- ASP.NET Core middleware
- Manages correlation ID lifecycle
- Reads from request headers or generates new ID
- Adds correlation ID to response headers

### Extensions (`Extensions/`)

**ObservabilityServiceCollectionExtensions.cs**
- Dependency injection configuration
- Service registration methods
- Serilog configuration
- Multiple configuration overloads

**ObservabilityApplicationBuilderExtensions.cs**
- Middleware pipeline configuration
- Request logging setup
- Context enrichment

## Sample Application

### Controllers

**UsersController.cs**
- Demonstrates basic logging patterns
- Shows error logging
- Includes sensitive data masking example

**ProductsController.cs**
- Demonstrates log scopes
- Shows async operations logging
- Includes pagination and search examples

### Configuration Files

**appsettings.json**
- Production configuration
- JSON format logging
- File rotation settings
- Sensitive data masking

**appsettings.Development.json**
- Development overrides
- Text format logging
- More verbose logging
- Easier debugging

## Documentation Files

### README.md
Main project documentation including:
- Features overview
- Installation instructions
- Quick start guide
- Configuration reference
- Advanced usage examples
- Architecture overview

### QUICKSTART.md
5-minute getting started guide:
- Minimal setup steps
- Basic configuration
- First controller example
- Common configurations

### USAGE_GUIDE.md
Comprehensive usage documentation:
- Detailed configuration options
- Best practices
- Advanced scenarios
- Troubleshooting
- Migration guides

### CONTRIBUTING.md
Contribution guidelines:
- Code of conduct
- Development setup
- Coding standards
- Pull request process
- Testing guidelines

### CHANGELOG.md
Version history:
- Release notes
- Feature additions
- Bug fixes
- Breaking changes

## Key Design Patterns

### Dependency Injection
All components are registered in the DI container and follow constructor injection pattern.

### Extension Methods
Fluent API for easy configuration:
```csharp
builder.Host.UseObservability(builder.Configuration);
builder.Services.AddObservability(builder.Configuration);
app.UseObservability();
```

### Options Pattern
Configuration uses the .NET Options pattern:
```csharp
public class ObservabilityOptions { ... }
```

### Interface Segregation
Small, focused interfaces for extensibility:
- `ICorrelationIdProvider`
- `ISensitiveDataMasker`
- `ILogEventEnricher` (Serilog)

### Middleware Pattern
ASP.NET Core middleware for cross-cutting concerns:
- Correlation ID management
- Request logging

## Technology Stack

### Core Dependencies
- **.NET 8.0** - Target framework
- **Serilog.AspNetCore 8.0.3** - Logging framework
- **Serilog.Enrichers.Environment 3.0.1** - Environment enrichment
- **Serilog.Enrichers.Thread 4.0.0** - Thread enrichment
- **Microsoft.AspNetCore.Http.Abstractions 2.2.0** - HTTP abstractions

### Development Tools
- Visual Studio / VS Code / Rider
- Git for version control
- .NET CLI for building and testing

## Build Artifacts

### Debug Build
```
bin/Debug/net8.0/
├── MonitoringSystem.Observability.dll
├── MonitoringSystem.Observability.pdb
└── MonitoringSystem.Observability.xml  (documentation)
```

### Release Build
```
bin/Release/net8.0/
├── MonitoringSystem.Observability.dll
├── MonitoringSystem.Observability.pdb
└── MonitoringSystem.Observability.xml  (documentation)
```

### NuGet Package (future)
```
bin/Release/
└── MonitoringSystem.Observability.1.0.0.nupkg
```

## Configuration Flow

1. **appsettings.json** → Loaded by ASP.NET Core
2. **IConfiguration** → Passed to UseObservability()
3. **ObservabilityOptions** → Bound from configuration
4. **LoggerConfiguration** → Configured with options
5. **Serilog Logger** → Created and registered
6. **ILogger<T>** → Injected into controllers/services

## Extensibility Points

### Custom Enrichers
Implement `ILogEventEnricher` and add to configuration.

### Custom Maskers
Implement `ISensitiveDataMasker` and register in DI.

### Custom Correlation ID Provider
Implement `ICorrelationIdProvider` and register in DI.

### Additional Sinks
Configure additional Serilog sinks in the configuration.

## Future Additions

Planned structure additions:
- `tests/` - Unit and integration tests
- `docs/` - Additional documentation and diagrams
- `benchmarks/` - Performance benchmarks
- `examples/` - More sample applications
- `.github/` - GitHub Actions workflows

## Naming Conventions

### Namespaces
- `MonitoringSystem.Observability` - Root namespace
- `MonitoringSystem.Observability.Configuration` - Configuration
- `MonitoringSystem.Observability.Enrichers` - Enrichers
- `MonitoringSystem.Observability.Middleware` - Middleware
- `MonitoringSystem.Observability.Extensions` - Extensions

### Files
- PascalCase for all file names
- Match class name: `CorrelationIdEnricher.cs`
- One class per file (except nested classes)

### Folders
- PascalCase for folder names
- Plural for collections: `Controllers/`, `Enrichers/`
- Singular for single purpose: `Configuration/`, `Middleware/`

## Version Control

### Ignored Files (.gitignore)
- Build outputs (`bin/`, `obj/`)
- User-specific files (`.vs/`, `.idea/`)
- Log files (`logs/`, `*.log`)
- NuGet packages (`*.nupkg`)

### Tracked Files
- Source code (`.cs`, `.csproj`)
- Configuration (`.json`)
- Documentation (`.md`)
- Solution file (`.sln`)
- License (`LICENSE`)

## Package Structure (NuGet)

When published as a NuGet package:
```
MonitoringSystem.Observability.1.0.0.nupkg
├── lib/
│   └── net8.0/
│       ├── MonitoringSystem.Observability.dll
│       └── MonitoringSystem.Observability.xml
├── README.md
└── LICENSE
```

## Summary

This project follows standard .NET project structure with clear separation of concerns:
- **Core library** in `src/`
- **Sample applications** in `samples/`
- **Tests** in `tests/` (future)
- **Documentation** at root level

The structure is designed for:
- Easy navigation
- Clear responsibilities
- Extensibility
- Maintainability
- NuGet packaging
