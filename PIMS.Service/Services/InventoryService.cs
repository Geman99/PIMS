using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PIMS.EntityFramework.Models;

namespace PIMS.Services
{
    public class InventoryService : IInventoryService
    {
        private readonly PimsDbContext _context;
        private readonly ILogger<InventoryService> _logger; // For logging

        public InventoryService(PimsDbContext context, ILogger<InventoryService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Inventory> GetInventoryByProductIdAsync(Guid productId)
        {
            return await _context.Inventories.FirstOrDefaultAsync(i => i.ProductID == productId) ?? new Inventory();
        }

        public async Task<Inventory> UpdateInventoryAsync(Guid productId, int quantityChange, string reason, Guid userId)
        {
            using (var transaction = _context.Database.BeginTransaction()) // Transaction for data integrity
            {
                try
                {
                    var inventory = await GetInventoryByProductIdAsync(productId);
                    if (inventory == null)
                    {
                        inventory = new Inventory { ProductID = productId, Quantity = 0, WarehouseLocation = "Default" }; // Or handle appropriately
                        _context.Inventories.Add(inventory); // Add if it doesn't exist
                    }

                    inventory.Quantity += quantityChange;
                    if (inventory.Quantity < 0)
                    {
                        throw new InvalidOperationException("Quantity cannot be negative."); // Or custom exception
                    }

                    var transactionRecord = new InventoryTransaction
                    {
                        InventoryId = inventory.InventoryID,  // Use the actual ID after adding/finding
                        QuantityChange = quantityChange,
                        Reason = reason,
                        UserId = userId
                    };
                    _context.InventoryTransactions.Add(transactionRecord);

                    await _context.SaveChangesAsync();
                    transaction.Commit();

                    _logger.LogInformation($"Inventory updated for Product {productId}: Quantity change {quantityChange}, Reason: {reason}, User: {userId}");

                    return inventory;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    _logger.LogError(ex, $"Error updating inventory for Product {productId}");
                    throw; // Re-throw the exception after logging
                }
            }
        }

        public async Task<List<InventoryTransaction>> GetInventoryTransactionsAsync(Guid productId)
        {
            var inventory = await GetInventoryByProductIdAsync(productId);
            if (inventory == null) return new List<InventoryTransaction>(); // Or handle appropriately

            return await _context.InventoryTransactions
                .Include(it => it.User) // Include user details
                .Where(it => it.InventoryId == inventory.InventoryID)
                .OrderByDescending(it => it.TransactionDate)
                .ToListAsync();
        }

        public async Task<bool> PerformInventoryAuditAsync(Guid productId, int newQuantity, string reason, Guid userId)
        {
            try
            {
                var updatedInventory = await UpdateInventoryAsync(productId, newQuantity - (await GetInventoryByProductIdAsync(productId))?.Quantity ?? 0, reason + " (Audit)", userId);
                return updatedInventory != null; // Return true if the update was successful (inventory found and updated)
            }
            catch (Exception ex) // Catch potential exceptions
            {
                _logger.LogError(ex, "Error performing inventory audit."); // Log the error
                return false; // Return false if there was an error
            }
        }

        public async Task<bool> CheckLowStockAsync(Guid productId, int threshold)
        {
            var inventory = await GetInventoryByProductIdAsync(productId);
            return inventory != null && inventory.Quantity < threshold;
        }
    }
}