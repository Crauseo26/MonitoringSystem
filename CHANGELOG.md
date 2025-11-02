# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [1.0.0] - 2025-10-13

### Added
- Initial release of MonitoringSystem.Observability
- Structured JSON logging with Serilog
- Correlation ID tracking for distributed systems
- Automatic HTTP request/response logging
- Sensitive data masking with configurable strategies
- Console and file logging sinks
- Log file rotation and retention policies
- Configurable log levels and namespace overrides
- Log enrichment (machine name, thread ID, environment, application name)
- Extensible architecture for custom enrichers and maskers
- Comprehensive configuration via appsettings.json
- Sample API demonstrating all features
- Complete documentation and usage guides

### Features

#### Core Logging
- Serilog-based structured logging
- JSON and text output formats
- Multiple log levels (Verbose, Debug, Information, Warning, Error, Fatal)
- Namespace-specific log level overrides
- Log scopes for contextual logging

#### Correlation ID Management
- Automatic correlation ID generation
- X-Correlation-ID header support
- Correlation ID in all log entries
- Correlation ID in response headers
- Custom correlation ID provider support

#### Sensitive Data Protection
- Automatic masking of sensitive properties
- Configurable property names to mask
- Custom masking strategies
- Partial masking support (show last N characters)

#### File Logging
- Configurable output directory
- Rolling file intervals (Infinite, Year, Month, Day, Hour, Minute)
- File size limits
- Retention policies
- Shared file access for multi-process scenarios

#### Request Logging
- Automatic HTTP request/response logging
- Request method, path, and status code
- Response time tracking
- Client IP address and User-Agent
- User authentication information
- Custom request enrichment

#### Extensibility
- Custom log enrichers
- Custom sensitive data maskers
- Custom correlation ID providers
- Pluggable architecture

### Dependencies
- .NET 8.0
- Serilog.AspNetCore 8.0.3
- Serilog.Enrichers.Environment 3.0.1
- Serilog.Enrichers.Thread 4.0.0
- Microsoft.AspNetCore.Http.Abstractions 2.2.0

### Documentation
- Comprehensive README with quick start guide
- Detailed usage guide with best practices
- Sample API with multiple endpoints
- Configuration examples for different environments
- Troubleshooting guide
- Migration guide from other logging frameworks

## [Unreleased]

### Planned Features
- [ ] NuGet package publication
- [ ] OpenTelemetry integration for metrics
- [ ] OpenTelemetry integration for distributed tracing
- [ ] Additional sinks (Elasticsearch, Seq, Application Insights)
- [ ] Performance metrics collection
- [ ] Health check endpoints
- [ ] Log sampling for high-volume scenarios
- [ ] Async logging optimization
- [ ] Structured exception logging
- [ ] Log filtering by HTTP status code
- [ ] Custom log formatters
- [ ] Log aggregation helpers
- [ ] Dashboard integration examples
