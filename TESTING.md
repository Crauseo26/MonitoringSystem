# Testing Guide

Instructions for testing the MonitoringSystem.Observability package.

## Running the Sample API

### 1. Start the Application

```bash
cd samples/SampleApi
dotnet run
```

The API will start on:
- HTTPS: `https://localhost:7XXX`
- HTTP: `http://localhost:5XXX`

(Replace XXX with the actual port numbers shown in the console)

### 2. Access Swagger UI

Open your browser and navigate to:
```
https://localhost:7XXX/swagger
```

## Manual Testing Scenarios

### Test 1: Basic Logging

**Endpoint:** `GET /api/users`

```bash
curl https://localhost:7XXX/api/users
```

**Expected:**
- Response with user list
- Console logs showing the request
- Log file created in `./logs/` directory
- Correlation ID in response headers

**Verify:**
```bash
# Check response headers
curl -i https://localhost:7XXX/api/users

# Look for: X-Correlation-ID header
```

### Test 2: Correlation ID Tracking

**Send request with custom correlation ID:**

```bash
curl -H "X-Correlation-ID: test-correlation-123" https://localhost:7XXX/api/users/1
```

**Expected:**
- Same correlation ID returned in response headers
- All logs include `test-correlation-123`

**Verify in logs:**
```bash
# Check log file
cat logs/log-*.txt | grep "test-correlation-123"
```

### Test 3: Structured Logging

**Endpoint:** `GET /api/users/1`

```bash
curl https://localhost:7XXX/api/users/1
```

**Expected:**
- Structured log entries with UserId property
- JSON format in log files (if UseJsonFormat: true)

**Verify in logs:**
```json
{
  "@t": "2025-10-13T...",
  "@mt": "Fetching user with ID: {UserId}",
  "UserId": 1,
  "CorrelationId": "...",
  "Application": "SampleApi"
}
```

### Test 4: Sensitive Data Masking

**Endpoint:** `POST /api/users`

```bash
curl -X POST https://localhost:7XXX/api/users \
  -H "Content-Type: application/json" \
  -d '{
    "name": "John Doe",
    "email": "john@example.com",
    "password": "SuperSecret123!"
  }'
```

**Expected:**
- User created successfully
- Password masked in logs as `***`

**Verify in logs:**
```bash
# Search for password in logs - should be masked
cat logs/log-*.txt | grep -i password
```

### Test 5: Error Logging

**Endpoint:** `GET /api/users/error`

```bash
curl https://localhost:7XXX/api/users/error
```

**Expected:**
- 500 Internal Server Error response
- Full exception details in logs
- Stack trace included

**Verify in logs:**
```bash
# Check for error logs
cat logs/log-*.txt | grep -A 10 "Error"
```

### Test 6: Log Scopes

**Endpoint:** `GET /api/products?page=2&pageSize=5`

```bash
curl "https://localhost:7XXX/api/products?page=2&pageSize=5"
```

**Expected:**
- All logs within the operation include Page and PageSize properties
- Structured context in log entries

**Verify in logs:**
Look for logs with Page and PageSize properties.

### Test 7: Request Logging

**Any endpoint:**

```bash
curl https://localhost:7XXX/api/users/1
```

**Expected:**
- Automatic HTTP request log with:
  - Method (GET)
  - Path (/api/users/1)
  - Status code (200)
  - Elapsed time (in ms)
  - Client IP
  - User-Agent

**Verify in logs:**
```
HTTP GET /api/users/1 responded 200 in 45.67 ms
```

### Test 8: Different Log Levels

**Test various endpoints:**

```bash
# Information level
curl https://localhost:7XXX/api/users

# Warning level
curl https://localhost:7XXX/api/users/0

# Error level
curl https://localhost:7XXX/api/users/error
```

**Expected:**
- Different log levels in output
- Appropriate severity indicators

### Test 9: Async Operations

**Endpoint:** `GET /api/products/1`

```bash
curl https://localhost:7XXX/api/products/1
```

**Expected:**
- Correlation ID maintained across async operations
- All logs include the same correlation ID

### Test 10: Search with Multiple Parameters

**Endpoint:** `GET /api/products/search`

```bash
curl "https://localhost:7XXX/api/products/search?query=laptop&minPrice=100&maxPrice=1000"
```

**Expected:**
- Log scope includes all query parameters
- Structured logging of search criteria

## Configuration Testing

### Test Different Environments

**Development:**
```bash
dotnet run --environment Development
```

**Production:**
```bash
dotnet run --environment Production
```

**Verify:**
- Different log formats (text vs JSON)
- Different log levels
- Environment property in logs

### Test Configuration Changes

Edit `appsettings.json` and test:

1. **Disable Console Logging:**
   ```json
   "EnableConsole": false
   ```
   Verify: No console output

2. **Change Log Level:**
   ```json
   "MinimumLevel": "Warning"
   ```
   Verify: Only warnings and errors appear

3. **Disable Sensitive Data Masking:**
   ```json
   "EnableMasking": false
   ```
   Verify: Passwords appear in logs (don't do this in production!)

4. **Change File Path:**
   ```json
   "Path": "./custom-logs"
   ```
   Verify: Logs written to new directory

## Log File Verification

### Check Log Files

```bash
# List log files
ls -la logs/

# View latest log
cat logs/log-$(date +%Y%m%d).txt

# Search for specific correlation ID
grep "correlation-id-here" logs/log-*.txt

# Count log entries
grep -c "@t" logs/log-*.txt  # For JSON format
```

### Verify Log Rotation

1. Run the application for multiple days
2. Check that new log files are created daily
3. Verify old logs are retained according to configuration

### Verify File Size Limits

1. Generate many log entries
2. Check that files don't exceed configured size
3. Verify new files are created when limit is reached

## Performance Testing

### Load Test

Use a tool like Apache Bench or k6:

```bash
# Simple load test
ab -n 1000 -c 10 https://localhost:7XXX/api/users
```

**Verify:**
- All requests have correlation IDs
- No performance degradation
- Logs are written correctly under load

### Correlation ID Under Load

```bash
# Multiple concurrent requests
for i in {1..100}; do
  curl -H "X-Correlation-ID: load-test-$i" https://localhost:7XXX/api/users/$i &
done
wait
```

**Verify:**
- Each request maintains its own correlation ID
- No correlation ID mixing between requests

## Integration Testing Checklist

- [ ] Application starts successfully
- [ ] Swagger UI is accessible
- [ ] All endpoints return expected responses
- [ ] Logs appear in console (Development)
- [ ] Logs written to files
- [ ] Correlation IDs are generated
- [ ] Custom correlation IDs are preserved
- [ ] Sensitive data is masked
- [ ] Errors are logged with stack traces
- [ ] Request logging includes timing
- [ ] Log scopes work correctly
- [ ] Different log levels work
- [ ] Configuration changes take effect
- [ ] File rotation works
- [ ] Multiple environments work correctly

## Automated Testing (Future)

### Unit Tests

```csharp
[Fact]
public void CorrelationIdEnricher_AddsCorrelationId()
{
    // Arrange
    var provider = new AsyncLocalCorrelationIdProvider();
    provider.SetCorrelationId("test-id");
    var enricher = new CorrelationIdEnricher(provider);
    
    // Act & Assert
    // ...
}
```

### Integration Tests

```csharp
[Fact]
public async Task Api_IncludesCorrelationIdInResponse()
{
    // Arrange
    var client = _factory.CreateClient();
    
    // Act
    var response = await client.GetAsync("/api/users/1");
    
    // Assert
    Assert.True(response.Headers.Contains("X-Correlation-ID"));
}
```

## Troubleshooting Tests

### Logs Not Appearing

1. Check console output for errors
2. Verify `UseObservability()` is called
3. Check log level configuration
4. Verify file permissions

### Correlation ID Issues

1. Check middleware order in Program.cs
2. Verify `ICorrelationIdProvider` is registered
3. Check that enricher is configured

### Masking Not Working

1. Verify property names in configuration
2. Check that `EnableMasking` is true
3. Ensure structured logging is used (not string interpolation)

## Success Criteria

All tests pass when:
- ✅ Application starts without errors
- ✅ All endpoints respond correctly
- ✅ Logs appear in both console and files
- ✅ Correlation IDs work correctly
- ✅ Sensitive data is masked
- ✅ Different environments work
- ✅ Configuration changes take effect
- ✅ Performance is acceptable under load

## Next Steps

After manual testing:
1. Create automated unit tests
2. Create integration tests
3. Set up CI/CD pipeline
4. Add performance benchmarks
5. Create load testing scenarios
