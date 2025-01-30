using Microsoft.EntityFrameworkCore;
using PIMS.EntityFramework.Models;
using PIMS.Services.Interfaces;

namespace PIMS.Services
{
    public class ProductService : IProductService
    {
        private readonly PimsDbContext _context;

        public ProductService(PimsDbContext context)
        {
            _context = context;
        }

        public async Task<Product> GetProductByIdAsync(Guid id)
        {
            return await _context.Products
                .Include(p => p.ProductCategories)
                .ThenInclude(pc => pc.Category)
                .FirstOrDefaultAsync(p => p.ProductID == id) ?? new Product();
        }

        public async Task<Product> CreateProductAsync(Product product, List<Guid> categoryIds)
        {
            if (await _context.Products.AnyAsync(p => p.SKU == product.SKU))
            {
                throw new ArgumentException("SKU already exists.");
            }

            // Add categories
            foreach (var catId in categoryIds)
            {
                var category = await _context.Categories.FindAsync(catId); // Fetch the category from the database
                if (category == null)
                {
                    throw new ArgumentException($"Category with ID {catId} not found."); // Handle missing category
                }

                product.ProductCategories.Add(new ProductCategory { CategoryId = catId, Category = category });
            }

            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            return product;
        }

        public async Task<List<Product>> GetAllProductsAsync(string searchTerm = null, List<Guid> categoryFilter = null)
        {
            IQueryable<Product> query = _context.Products.Include(p => p.ProductCategories).ThenInclude(pc => pc.Category);

            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(p => p.Name.Contains(searchTerm) || p.Description.Contains(searchTerm) || p.SKU.Contains(searchTerm));
            }

            if (categoryFilter != null && categoryFilter.Any())
            {
                query = query.Where(p => p.ProductCategories.Any(pc => categoryFilter.Contains(pc.CategoryId)));
            }

            return await query.ToListAsync();
        }

        public async Task<Product> UpdateProductAsync(Guid id, Product product, List<Guid> categoryIds)
        {
            var existingProduct = await _context.Products.FirstOrDefaultAsync(p => p.ProductID == id);

            if (existingProduct == null) return null; // Or throw an exception

            if (await _context.Products.AnyAsync(p => p.SKU == product.SKU && p.ProductID != id))
            {
                throw new ArgumentException("SKU already exists.");
            }

            existingProduct.Name = product.Name;
            existingProduct.Description = product.Description;
            existingProduct.Price = product.Price;
            existingProduct.SKU = product.SKU;

            // Update categories (more efficient approach)
            _context.ProductCategories.RemoveRange(existingProduct.ProductCategories); // Clear existing
            foreach (var catId in categoryIds)
            {
                existingProduct.ProductCategories.Add(new ProductCategory { ProductId = id, CategoryId = catId });
            }

            await _context.SaveChangesAsync();
            return existingProduct;
        }

        public async Task<bool> DeleteProductAsync(Guid id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null) return false;

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> AdjustPriceAsync(Guid productId, decimal percentage)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product == null) return false;

            decimal newPrice = product.Price * (1 - percentage / 100);
            if (newPrice < 0) return false; // Prevent negative prices

            product.Price = newPrice;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> IsSKUUniqueAsync(string sku)
        {
            return !await _context.Products.AnyAsync(p => p.SKU == sku);
        }
    }
}