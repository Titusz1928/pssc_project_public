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
        
        // Retrieve a product by its ID
        public async Task<List<Code>> GetExistingProductsAsync(IEnumerable<string> codesToCheck)
        {
            // Fetch all products from the database, but defer filtering to in-memory evaluation
            List<ProductDto> products = await Task.Run(() =>
                dbContext.Products
                    .AsNoTracking()
                    .AsEnumerable()  // Switch to in-memory evaluation
                    .Where(product => codesToCheck.Contains(product.Code))  // In-memory filtering
                    .ToList());  // Convert to list after in-memory filtering

            return products.Select(product => new Code(product.Code)).ToList();
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
