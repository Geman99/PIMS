namespace PIMS.EntityFramework.Models
{
    public class Product
    {
        public Guid ProductID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string SKU { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        public List<ProductCategory> ProductCategories { get; set; } = new(); // For EF Core
        public List<Category> Categories => ProductCategories?.Select(pc => pc.Category).ToList() ?? new(); // For easy access
    }
}