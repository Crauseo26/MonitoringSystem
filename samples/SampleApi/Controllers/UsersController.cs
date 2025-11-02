using Microsoft.AspNetCore.Mvc;

namespace SampleApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly ILogger<UsersController> _logger;

    public UsersController(ILogger<UsersController> logger)
    {
        _logger = logger;
    }

    [HttpGet]
    public IActionResult GetUsers()
    {
        _logger.LogInformation("Fetching all users");

        var users = new[]
        {
            new { Id = 1, Name = "John Doe", Email = "john@example.com" },
            new { Id = 2, Name = "Jane Smith", Email = "jane@example.com" },
            new { Id = 3, Name = "Bob Johnson", Email = "bob@example.com" }
        };

        _logger.LogInformation("Retrieved {UserCount} users", users.Length);

        return Ok(users);
    }

    [HttpGet("{id}")]
    public IActionResult GetUser(int id)
    {
        _logger.LogInformation("Fetching user with ID: {UserId}", id);

        if (id <= 0)
        {
            _logger.LogWarning("Invalid user ID provided: {UserId}", id);
            return BadRequest(new { Error = "Invalid user ID" });
        }

        if (id > 100)
        {
            _logger.LogWarning("User not found with ID: {UserId}", id);
            return NotFound(new { Error = "User not found" });
        }

        var user = new { Id = id, Name = $"User {id}", Email = $"user{id}@example.com" };
        _logger.LogInformation("Successfully retrieved user: {@User}", user);

        return Ok(user);
    }

    [HttpPost]
    public IActionResult CreateUser([FromBody] CreateUserRequest request)
    {
        _logger.LogInformation("Creating new user with email: {Email}", request.Email);

        // Simulate validation
        if (string.IsNullOrEmpty(request.Name))
        {
            _logger.LogWarning("User creation failed: Name is required");
            return BadRequest(new { Error = "Name is required" });
        }

        // Log with sensitive data (password will be masked)
        _logger.LogDebug("User creation request: {@Request}", request);

        var newUser = new
        {
            Id = Random.Shared.Next(1, 1000),
            Name = request.Name,
            Email = request.Email,
            CreatedAt = DateTime.UtcNow
        };

        _logger.LogInformation("User created successfully with ID: {UserId}", newUser.Id);

        return CreatedAtAction(nameof(GetUser), new { id = newUser.Id }, newUser);
    }

    [HttpDelete("{id}")]
    public IActionResult DeleteUser(int id)
    {
        _logger.LogInformation("Attempting to delete user with ID: {UserId}", id);

        if (id <= 0)
        {
            _logger.LogWarning("Invalid user ID for deletion: {UserId}", id);
            return BadRequest(new { Error = "Invalid user ID" });
        }

        _logger.LogInformation("User deleted successfully: {UserId}", id);

        return NoContent();
    }

    [HttpGet("error")]
    public IActionResult SimulateError()
    {
        _logger.LogWarning("Simulating an error scenario");

        try
        {
            throw new InvalidOperationException("This is a simulated error for testing logging");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while processing the request");
            return StatusCode(500, new { Error = "Internal server error", Message = ex.Message });
        }
    }
}

public record CreateUserRequest(string Name, string Email, string Password);
