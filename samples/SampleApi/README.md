# Sample API - Observability Demo

This sample API demonstrates the usage of the MonitoringSystem.Observability library.

## Features Demonstrated

1. **Structured Logging** - All endpoints use structured logging with named parameters
2. **Correlation ID Tracking** - Every request gets a correlation ID
3. **Request Logging** - Automatic HTTP request/response logging
4. **Sensitive Data Masking** - Password fields are automatically masked
5. **Log Scopes** - ProductsController demonstrates using log scopes
6. **Error Logging** - Error endpoint shows exception logging
7. **Different Log Levels** - Examples of Information, Warning, Error, and Debug logs

## Running the Sample

1. Navigate to the sample directory:
   ```bash
   cd samples/SampleApi
   ```

2. Run the application:
   ```bash
   dotnet run
   ```

3. The API will start on:
   - HTTPS: https://localhost:7XXX
   - HTTP: http://localhost:5XXX

## Available Endpoints

### Users API

- `GET /api/users` - Get all users
- `GET /api/users/{id}` - Get user by ID
- `POST /api/users` - Create new user (demonstrates sensitive data masking)
- `DELETE /api/users/{id}` - Delete user
- `GET /api/users/error` - Simulate an error (demonstrates error logging)

### Products API

- `GET /api/products` - Get products with pagination
- `GET /api/products/{id}` - Get product by ID
- `POST /api/products` - Create new product
- `PUT /api/products/{id}` - Update product
- `GET /api/products/search` - Search products (demonstrates log scopes)

### Weather Forecast API

- `GET /weatherforecast` - Get weather forecast (default endpoint)

## Testing Examples

### Test Correlation ID

```bash
# Send request with custom correlation ID
curl -H "X-Correlation-ID: my-custom-id-123" https://localhost:7XXX/api/users

# Check response headers - should include X-Correlation-ID
```

### Test Sensitive Data Masking

```bash
# Create user with password
curl -X POST https://localhost:7XXX/api/users \
  -H "Content-Type: application/json" \
  -d '{
    "name": "John Doe",
    "email": "john@example.com",
    "password": "SuperSecret123!"
  }'

# Check logs - password should be masked as "***"
```

### Test Error Logging

```bash
# Trigger an error
curl https://localhost:7XXX/api/users/error

# Check logs - should include full exception details
```

### Test Pagination with Scopes

```bash
# Get products with pagination
curl "https://localhost:7XXX/api/products?page=2&pageSize=5"

# Check logs - should include Page and PageSize in log scope
```

## Log Locations

Logs are written to:
- **Console**: Real-time output (text format in Development, JSON in Production)
- **Files**: `./logs/log-YYYYMMDD.txt`

## Configuration

### Development (appsettings.Development.json)
- Log Level: Debug
- Format: Text (human-readable)
- Console: Enabled
- File: Enabled

### Production (appsettings.json)
- Log Level: Information
- Format: JSON (structured)
- Console: Enabled
- File: Enabled
- File Retention: 31 days
- Max File Size: 100 MB

## Observing Logs

### Console Output (Development)

```
[21:30:00 INF] HTTP GET /api/users responded 200 in 45.67 ms
[21:30:05 INF] Fetching user with ID: 1
[21:30:05 INF] Successfully retrieved user: {"Id": 1, "Name": "User 1", "Email": "user1@example.com"}
```

### File Output (JSON)

```json
{
  "@t": "2025-10-13T21:30:00.1234567Z",
  "@l": "Information",
  "@mt": "Fetching user with ID: {UserId}",
  "UserId": 1,
  "CorrelationId": "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
  "Application": "SampleApi",
  "Environment": "Production",
  "MachineName": "MY-MACHINE",
  "ThreadId": 12,
  "SourceContext": "SampleApi.Controllers.UsersController"
}
```

## Key Code Patterns

### Basic Logging

```csharp
_logger.LogInformation("Fetching user with ID: {UserId}", id);
```

### Structured Logging with Objects

```csharp
_logger.LogInformation("Successfully retrieved user: {@User}", user);
```

### Using Scopes

```csharp
using (_logger.BeginScope(new Dictionary<string, object>
{
    ["Page"] = page,
    ["PageSize"] = pageSize
}))
{
    _logger.LogInformation("Processing request");
    // All logs here will include Page and PageSize
}
```

### Error Logging

```csharp
try
{
    // Code that might throw
}
catch (Exception ex)
{
    _logger.LogError(ex, "An error occurred while processing {Operation}", operation);
    throw;
}
```

## Customization

You can modify `appsettings.json` to experiment with different configurations:

- Change log levels
- Enable/disable console or file logging
- Switch between JSON and text format
- Adjust file retention policies
- Add custom properties to mask

## Next Steps

1. Explore the controller code to see logging patterns
2. Modify configuration and observe changes
3. Add your own endpoints with logging
4. Experiment with custom enrichers
5. Implement custom sensitive data maskers

## Troubleshooting

### Logs not appearing in files
- Check that the `./logs` directory has write permissions
- Verify `EnableFile` is set to `true` in configuration

### Sensitive data not being masked
- Ensure property names match those in `PropertyNamesToMask` configuration
- Check that `EnableMasking` is set to `true`

### Correlation ID not in logs
- Verify that `UseObservability()` is called in the middleware pipeline
- Check that the correlation ID provider is registered in DI
