using PIMS.EntityFramework.Models;

namespace PIMS.Services.Interfaces
{
    public interface IProductService
    {
        Task<Product> CreateProductAsync(Product product, List<Guid> categoryIds);

        Task<Product> GetProductByIdAsync(Guid id);

        Task<List<Product>> GetAllProductsAsync(string searchTerm = null, List<Guid> categoryFilter = null);

        Task<Product> UpdateProductAsync(Guid id, Product product, List<Guid> categoryIds);

        Task<bool> DeleteProductAsync(Guid id);

        Task<bool> AdjustPriceAsync(Guid productId, decimal percentage);

        Task<bool> IsSKUUniqueAsync(string sku);
    }
}