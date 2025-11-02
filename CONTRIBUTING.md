# Contributing to MonitoringSystem.Observability

Thank you for your interest in contributing! This document provides guidelines and instructions for contributing to this project.

## Code of Conduct

- Be respectful and inclusive
- Provide constructive feedback
- Focus on what is best for the community
- Show empathy towards other community members

## How to Contribute

### Reporting Bugs

If you find a bug, please create an issue with:

1. **Clear title and description**
2. **Steps to reproduce** the issue
3. **Expected behavior** vs **actual behavior**
4. **Environment details** (OS, .NET version, etc.)
5. **Code samples** or **log output** if applicable

### Suggesting Enhancements

Enhancement suggestions are welcome! Please create an issue with:

1. **Clear description** of the enhancement
2. **Use case** - why is this needed?
3. **Proposed solution** (if you have one)
4. **Alternatives considered**

### Pull Requests

1. **Fork the repository**
2. **Create a feature branch** (`git checkout -b feature/amazing-feature`)
3. **Make your changes**
4. **Add tests** if applicable
5. **Update documentation** as needed
6. **Commit your changes** (`git commit -m 'Add amazing feature'`)
7. **Push to the branch** (`git push origin feature/amazing-feature`)
8. **Open a Pull Request**

## Development Setup

### Prerequisites

- .NET 8.0 SDK or later
- Git
- IDE (Visual Studio, VS Code, or Rider)

### Getting Started

1. Clone the repository:
   ```bash
   git clone https://github.com/Crauseo26/MonitoringSystem.git
   cd MonitoringSystem
   ```

2. Restore dependencies:
   ```bash
   dotnet restore
   ```

3. Build the solution:
   ```bash
   dotnet build
   ```

4. Run the sample API:
   ```bash
   cd samples/SampleApi
   dotnet run
   ```

## Coding Standards

### C# Style Guidelines

- Follow [Microsoft C# Coding Conventions](https://docs.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/coding-conventions)
- Use meaningful variable and method names
- Add XML documentation comments for public APIs
- Keep methods focused and concise
- Use `async/await` for asynchronous operations

### Example

```csharp
/// <summary>
/// Processes the user request and returns the result
/// </summary>
/// <param name="userId">The unique identifier of the user</param>
/// <returns>A task representing the asynchronous operation</returns>
public async Task<UserResult> ProcessUserRequestAsync(int userId)
{
    _logger.LogInformation("Processing request for user {UserId}", userId);
    
    // Implementation
    
    return result;
}
```

### Naming Conventions

- **Classes**: PascalCase (`UserController`, `ObservabilityOptions`)
- **Methods**: PascalCase (`GetUser`, `ProcessAsync`)
- **Properties**: PascalCase (`ApplicationName`, `EnableLogging`)
- **Private fields**: _camelCase (`_logger`, `_correlationIdProvider`)
- **Parameters**: camelCase (`userId`, `configurationSection`)
- **Local variables**: camelCase (`user`, `correlationId`)

### File Organization

```
MonitoringSystem.Observability/
├── Configuration/      # Configuration models
├── Enrichers/         # Log enrichers
├── Middleware/        # ASP.NET Core middleware
├── Extensions/        # Extension methods
└── Models/           # Data models (if needed)
```

## Documentation

### Code Documentation

- Add XML documentation comments to all public classes, methods, and properties
- Include `<summary>`, `<param>`, `<returns>`, and `<exception>` tags as appropriate
- Provide meaningful descriptions, not just repetitions of the name

### README Updates

When adding new features:
- Update the main README.md with usage examples
- Update USAGE_GUIDE.md with detailed instructions
- Update CHANGELOG.md with the changes

## Testing

### Writing Tests

- Write unit tests for new functionality
- Ensure tests are isolated and repeatable
- Use descriptive test names: `MethodName_Scenario_ExpectedBehavior`
- Follow AAA pattern: Arrange, Act, Assert

### Running Tests

```bash
dotnet test
```

## Commit Messages

### Format

```
<type>(<scope>): <subject>

<body>

<footer>
```

### Types

- **feat**: New feature
- **fix**: Bug fix
- **docs**: Documentation changes
- **style**: Code style changes (formatting, etc.)
- **refactor**: Code refactoring
- **test**: Adding or updating tests
- **chore**: Maintenance tasks

### Examples

```
feat(enrichers): add custom property enricher

Added support for custom property enrichment through configuration.
This allows users to add static properties to all log events.

Closes #123
```

```
fix(middleware): correlation ID not propagating in async contexts

Fixed issue where correlation ID was lost in async operations.
Now using AsyncLocal for proper async context flow.

Fixes #456
```

## Pull Request Process

1. **Update documentation** for any changed functionality
2. **Add tests** for new features or bug fixes
3. **Ensure all tests pass** (`dotnet test`)
4. **Update CHANGELOG.md** with your changes
5. **Request review** from maintainers
6. **Address feedback** promptly
7. **Squash commits** if requested

### PR Title Format

Use the same format as commit messages:
```
feat(scope): description
fix(scope): description
docs: description
```

## Feature Requests

We welcome feature requests! Before submitting:

1. **Check existing issues** to avoid duplicates
2. **Provide context** - what problem does this solve?
3. **Consider alternatives** - are there other ways to achieve this?
4. **Be open to discussion** - the solution might evolve

### Priority Features

Current priorities:
- OpenTelemetry integration for metrics
- OpenTelemetry integration for distributed tracing
- Additional log sinks (Elasticsearch, Seq, etc.)
- Performance optimizations
- Enhanced filtering capabilities

## Questions?

If you have questions:
- Check the [README](README.md) and [USAGE_GUIDE](USAGE_GUIDE.md)
- Search existing issues
- Create a new issue with the "question" label

## License

By contributing, you agree that your contributions will be licensed under the MIT License.

## Recognition

Contributors will be recognized in:
- CHANGELOG.md for their contributions
- GitHub contributors page
- Release notes

Thank you for contributing! 🎉
