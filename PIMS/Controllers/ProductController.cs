using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PIMS.EntityFramework.Models;
using PIMS.Model.DTO;
using PIMS.Services.Interfaces;

namespace PIMS.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/products")]
    //[Authorize(Roles = "Admin,User")] // Example RBAC
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly ILogger<ProductController> _logger;

        public ProductController(IProductService productService, ILogger<ProductController> logger)
        {
            _productService = productService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts(string searchTerm = null, [FromQuery] List<Guid> categoryFilter = null)
        {
            var products = await _productService.GetAllProductsAsync(searchTerm, categoryFilter);
            return Ok(products);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetProduct(Guid id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null) return NotFound();
            return Ok(product);
        }

        [HttpPost]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateProduct(ProductCreateDto productDto) // Use DTO
        {
            var product = new Product
            {
                ProductID = Guid.NewGuid(),
                Name = productDto.Name,
                Description = productDto.Description,
                Price = productDto.Price,
                SKU = productDto.SKU
            };

            try
            {
                var createdProduct = await _productService.CreateProductAsync(product, productDto.CategoryIds);
                return Ok(createdProduct);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message); // Handle exceptions
            }
        }

        // ... (Implement other controller actions: Update, Delete, Price Adjustment)

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateProduct(Guid id, ProductUpdateDto productDto)
        {
            var product = new Product
            {
                Name = productDto.Name,
                Description = productDto.Description,
                Price = productDto.Price,
                SKU = productDto.SKU
            };

            try
            {
                var updatedProduct = await _productService.UpdateProductAsync(id, product, productDto.CategoryIds);
                return Ok(updatedProduct);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteProduct(Guid id)
        {
            var success = await _productService.DeleteProductAsync(id);
            if (!success) return NotFound();
            return NoContent();
        }

        [HttpPost("{id}/adjustprice")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AdjustPrice(Guid id, [FromBody] decimal percentage)
        {
            if (percentage <= 0) return BadRequest("Percentage must be greater than zero.");

            var success = await _productService.AdjustPriceAsync(id, percentage);
            if (!success) return NotFound();
            return Ok();
        }
    }
}