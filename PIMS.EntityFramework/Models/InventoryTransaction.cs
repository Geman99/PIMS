namespace PIMS.EntityFramework.Models
{
    public class InventoryTransaction
    {
        public Guid Id { get; set; }
        public Guid InventoryId { get; set; }
        public Inventory Inventory { get; set; }
        public int QuantityChange { get; set; } // Positive for addition, negative for subtraction
        public DateTime TransactionDate { get; set; } = DateTime.UtcNow;
        public string Reason { get; set; } // E.g., "Sale", "Restock", "Audit"
        public Guid UserId { get; set; } // Who performed the transaction
        public User User { get; set; }
    }
}