using System.Collections.Generic;
using System.Threading.Tasks;
using Lab2.Domain.Models;

namespace Lab2.Domain.Repositories
{
    public interface IProductsRepository
    {
        // Retrieve all products
        Task<List<Product>> GetAllProductsAsync();
        
        Task<List<ProductId>> GetExistingProductsAsync(IEnumerable<string> productIdsToCheck);


        // Retrieve a product by its ID
        Task<Product> GetProductByIdAsync(int productId);

        // Add a new product
        Task AddProductAsync(Product product);

        // Update an existing product
        Task UpdateProductAsync(Product product);

        // Delete a product by ID
        Task DeleteProductAsync(int productId);
    }
}