namespace PIMS.EntityFramework.Models
{
    public class Category
    {
        public Guid CategoryId { get; set; }
        public string Name { get; set; }
        public List<ProductCategory> ProductCategories { get; set; } = new();
        public List<Product> Products => ProductCategories?.Select(pc => pc.Product).ToList() ?? new();
    }
}