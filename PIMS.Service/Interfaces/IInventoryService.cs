using PIMS.EntityFramework.Models;
using PIMS.Model;

namespace PIMS.Services
{
    public interface IInventoryService
    {
        Task<Inventory> GetInventoryByProductIdAsync(Guid productId);
        Task<Inventory> UpdateInventoryAsync(Guid productId, int quantityChange, string reason, Guid userId); // Transactional
        Task<List<InventoryTransaction>> GetInventoryTransactionsAsync(Guid productId);
        Task<bool> PerformInventoryAuditAsync(Guid productId, int newQuantity, string reason, Guid userId);
        Task<bool> CheckLowStockAsync(Guid productId, int threshold); // Low stock check
    }
}