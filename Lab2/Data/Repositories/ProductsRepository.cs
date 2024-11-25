using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lab2.Data.Models;
using Lab2.Domain.Models;
using Lab2.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Lab2.Data.Repositories
{
    public class ProductsRepository : IProductsRepository
    {
        private readonly OrderLineContext dbContext;

        public ProductsRepository(OrderLineContext dbContext)
        {
            this.dbContext = dbContext;
        }

        // Retrieve all products
        public async Task<List<Product>> GetAllProductsAsync()
        {
            var productDtos = await dbContext.Products.AsNoTracking().ToListAsync();
            return productDtos.Select(dto => new Product(
                ProductId: dto.ProductId,
                Code: new ProductCode(dto.Code),
                Stoc: dto.Stoc)).ToList();
        }
        
        public async Task<List<ProductId>> GetExistingProductsAsync(IEnumerable<string> productIdsToCheck)
        {
            var productIdsList = productIdsToCheck.ToList();  // Ensure it's a list

            // Fetch products and filter them in memory
            var allProducts = await dbContext.Products.AsNoTracking().ToListAsync();  // Fetch all products
            var filteredProducts = allProducts.Where(p => productIdsList.Contains(p.ProductId.ToString())).ToList(); // Perform filtering in memory

            return filteredProducts.Select(p => new ProductId(p.ProductId)).ToList();
        }


        // Retrieve a product by its ID
        public async Task<Product> GetProductByIdAsync(int productId)
        {
            var productDto = await dbContext.Products.AsNoTracking().FirstOrDefaultAsync(p => p.ProductId == productId);
            return productDto == null ? null : new Product(
                ProductId: productDto.ProductId,
                Code: new ProductCode(productDto.Code),
                Stoc: productDto.Stoc);
        }

        // Add a new product
        public async Task AddProductAsync(Product product)
        {
            var productDto = new ProductDto
            {
                ProductId = product.ProductId,
                Code = product.Code.Value,
                Stoc = product.Stoc
            };
            dbContext.Products.Add(productDto);
            await dbContext.SaveChangesAsync();
        }

        // Update an existing product
        public async Task UpdateProductAsync(Product product)
        {
            var productDto = new ProductDto
            {
                ProductId = product.ProductId,
                Code = product.Code.Value,
                Stoc = product.Stoc
            };
            dbContext.Entry(productDto).State = EntityState.Modified;
            await dbContext.SaveChangesAsync();
        }

        // Delete a product by ID
        public async Task DeleteProductAsync(int productId)
        {
            var productDto = await dbContext.Products.FindAsync(productId);
            if (productDto != null)
            {
                dbContext.Products.Remove(productDto);
                await dbContext.SaveChangesAsync();
            }
        }
    }
}
