namespace PIMS.EntityFramework.Models
{
    public class Inventory
    {
        public Guid InventoryID { get; set; }
        public Guid ProductID { get; set; }
        public Product Product { get; set; }
        public int Quantity { get; set; }
        public string WarehouseLocation { get; set; }
    }
}