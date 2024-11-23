using System.Collections.Generic;
using System.Threading.Tasks;
using Lab2.Domain.Models;

namespace Lab2.Domain.Repositories
{
    public interface IProductsRepository
    {
        // Retrieve all products
        Task<List<Product>> GetAllProductsAsync();
        
        //Retrieve a product by its Code
        Task<List<Code>> GetExistingProductsAsync(IEnumerable<string> codesToCheck);


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