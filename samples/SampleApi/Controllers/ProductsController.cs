using Microsoft.AspNetCore.Mvc;

namespace SampleApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly ILogger<ProductsController> _logger;

    public ProductsController(ILogger<ProductsController> logger)
    {
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetProducts([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        using (_logger.BeginScope(new Dictionary<string, object>
        {
            ["Page"] = page,
            ["PageSize"] = pageSize,
            ["Operation"] = "GetProducts"
        }))
        {
            _logger.LogInformation("Fetching products - Page: {Page}, PageSize: {PageSize}", page, pageSize);

            // Simulate async operation
            await Task.Delay(100);

            var products = Enumerable.Range(1, pageSize).Select(i => new
            {
                Id = (page - 1) * pageSize + i,
                Name = $"Product {(page - 1) * pageSize + i}",
                Price = Random.Shared.Next(10, 1000),
                InStock = Random.Shared.Next(0, 2) == 1
            }).ToList();

            _logger.LogInformation("Retrieved {ProductCount} products", products.Count);

            return Ok(new
            {
                Page = page,
                PageSize = pageSize,
                TotalCount = 100,
                Data = products
            });
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetProduct(int id)
    {
        _logger.LogInformation("Fetching product with ID: {ProductId}", id);

        // Simulate database lookup
        await Task.Delay(50);

        if (id <= 0)
        {
            _logger.LogWarning("Invalid product ID: {ProductId}", id);
            return BadRequest(new { Error = "Invalid product ID" });
        }

        if (id > 100)
        {
            _logger.LogWarning("Product not found: {ProductId}", id);
            return NotFound(new { Error = "Product not found" });
        }

        var product = new
        {
            Id = id,
            Name = $"Product {id}",
            Description = $"Description for product {id}",
            Price = Random.Shared.Next(10, 1000),
            Category = "Electronics",
            InStock = true
        };

        _logger.LogInformation("Product retrieved successfully: {ProductId}", id);

        return Ok(product);
    }

    [HttpPost]
    public async Task<IActionResult> CreateProduct([FromBody] CreateProductRequest request)
    {
        using (_logger.BeginScope(new Dictionary<string, object>
        {
            ["Operation"] = "CreateProduct",
            ["ProductName"] = request.Name
        }))
        {
            _logger.LogInformation("Creating new product: {ProductName}", request.Name);

            // Validate
            if (string.IsNullOrEmpty(request.Name))
            {
                _logger.LogWarning("Product creation failed: Name is required");
                return BadRequest(new { Error = "Product name is required" });
            }

            if (request.Price <= 0)
            {
                _logger.LogWarning("Product creation failed: Invalid price {Price}", request.Price);
                return BadRequest(new { Error = "Price must be greater than zero" });
            }

            // Simulate async save
            await Task.Delay(100);

            var newProduct = new
            {
                Id = Random.Shared.Next(1, 10000),
                Name = request.Name,
                Description = request.Description,
                Price = request.Price,
                CreatedAt = DateTime.UtcNow
            };

            _logger.LogInformation("Product created successfully with ID: {ProductId}", newProduct.Id);

            return CreatedAtAction(nameof(GetProduct), new { id = newProduct.Id }, newProduct);
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateProduct(int id, [FromBody] UpdateProductRequest request)
    {
        _logger.LogInformation("Updating product {ProductId}", id);

        if (id <= 0)
        {
            _logger.LogWarning("Invalid product ID for update: {ProductId}", id);
            return BadRequest(new { Error = "Invalid product ID" });
        }

        // Simulate async update
        await Task.Delay(100);

        _logger.LogInformation("Product {ProductId} updated successfully", id);

        return Ok(new
        {
            Id = id,
            Name = request.Name,
            Description = request.Description,
            Price = request.Price,
            UpdatedAt = DateTime.UtcNow
        });
    }

    [HttpGet("search")]
    public async Task<IActionResult> SearchProducts([FromQuery] string query, [FromQuery] decimal? minPrice, [FromQuery] decimal? maxPrice)
    {
        using (_logger.BeginScope(new Dictionary<string, object>
        {
            ["SearchQuery"] = query ?? "null",
            ["MinPrice"] = minPrice ?? 0,
            ["MaxPrice"] = maxPrice ?? 0
        }))
        {
            _logger.LogInformation("Searching products with query: {Query}, MinPrice: {MinPrice}, MaxPrice: {MaxPrice}",
                query, minPrice, maxPrice);

            // Simulate search operation
            await Task.Delay(150);

            var results = Enumerable.Range(1, 5).Select(i => new
            {
                Id = i,
                Name = $"Product matching '{query}' - {i}",
                Price = Random.Shared.Next((int)(minPrice ?? 10), (int)(maxPrice ?? 1000))
            }).ToList();

            _logger.LogInformation("Search completed. Found {ResultCount} products", results.Count);

            return Ok(results);
        }
    }
}

public record CreateProductRequest(string Name, string Description, decimal Price);
public record UpdateProductRequest(string Name, string Description, decimal Price);
