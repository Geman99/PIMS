namespace PIMS.Model
{
    public class InventoryModel
    {
        public int InventoryID { get; set; }
        public int ProductID { get; set; }
        public int Quantity { get; set; }
        public string WarehouseLocation { get; set; }
    }
}