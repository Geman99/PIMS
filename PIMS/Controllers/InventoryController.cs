using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PIMS.EntityFramework.Models;
using PIMS.Services;
using System.Security.Claims;

namespace PIMS.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/inventory")]
    [Authorize(Roles = "Admin,User")] // Adjust roles as needed
    public class InventoryController : ControllerBase
    {
        private readonly IInventoryService _inventoryService;
        private readonly ILogger<InventoryController> _logger;

        public InventoryController(IInventoryService inventoryService, ILogger<InventoryController> logger)
        {
            _inventoryService = inventoryService;
            _logger = logger;
        }

        [HttpGet("{productId}")]
        public async Task<ActionResult<Inventory>> GetInventory(Guid productId)
        {
            var inventory = await _inventoryService.GetInventoryByProductIdAsync(productId);
            if (inventory == null) return NotFound();
            return Ok(inventory);
        }

        [HttpPost("{productId}/adjust")]
        [Authorize(Roles = "Admin")] // Example: Only admins can adjust inventory
        public async Task<IActionResult> AdjustInventory(Guid productId, int quantityChange, string reason)
        {
            try
            {
                Guid userId = GetCurrentUserId(); // Get user ID from claims
                if (userId == Guid.Empty) return Unauthorized();

                var updatedInventory = await _inventoryService.UpdateInventoryAsync(productId, quantityChange, reason, userId);
                return Ok(updatedInventory);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adjusting inventory.");
                return StatusCode(500, "An error occurred while adjusting inventory."); // Generic error message
            }
        }

        [HttpGet("{productId}/transactions")]
        public async Task<ActionResult<List<InventoryTransaction>>> GetTransactions(Guid productId)
        {
            var transactions = await _inventoryService.GetInventoryTransactionsAsync(productId);
            return Ok(transactions);
        }

        [HttpPost("{productId}/audit")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> PerformAudit(Guid productId, int quantityChange, string reason)
        {
            try
            {
                Guid userId = GetCurrentUserId(); // Get user ID from claims
                if (userId == Guid.Empty) return Unauthorized();

                var success = await _inventoryService.PerformInventoryAuditAsync(productId, quantityChange, reason, userId);
                if (!success) return BadRequest("Inventory audit failed."); // More specific message
                return Ok();
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error performing inventory audit.");
                return StatusCode(500, "An error occurred during the audit.");
            }
        }

        [HttpGet("{productId}/lowstock")]
        public async Task<ActionResult<bool>> CheckLowStock(Guid productId, [FromQuery] int threshold)
        {
            var isLowStock = await _inventoryService.CheckLowStockAsync(productId, threshold);
            return Ok(isLowStock);
        }

        // Helper method to get the current user's ID from JWT claims
        private Guid GetCurrentUserId()
        {
            try
            {
                var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (Guid.TryParse(userIdClaim, out Guid userId))
                {
                    return userId;
                }
                _logger.LogWarning("User ID not found in claims.");
                return Guid.Empty; // Or throw an exception if user ID is required
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user ID from claims.");
                return Guid.Empty; // Or throw an exception
            }
        }
    }
}