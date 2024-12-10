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
        
        public async Task<Quantity> GetAvailableStockAsync(IEnumerable<string> productIdsToCheck)
        {
            // First, fetch the products that match the product IDs into memory
            var products = await dbContext.Products
                .AsNoTracking()  // Avoid tracking for better performance
                .ToListAsync();  // Fetch all products to memory
    
            // Now, filter the products in memory (client-side)
            var product = products
                .FirstOrDefault(p => productIdsToCheck.Contains(p.ProductId.ToString()));

            // Return its quantity, or 0 if not found
            return new Quantity(product?.Stoc ?? 0);  // Use null-coalescing to return 0 if the product is not found
        }

        public async Task UpdateStockAsync(Order.IOrder payedOrder)
        {
            // Cast to PayedOrder to access the details of the order lines
            if (payedOrder is Order.PayedOrder payedOrderDetails)
            {
                foreach (var orderLine in payedOrderDetails.OrderList)
                {
                    // Fetch the product matching the ProductId from the database
                    var product = await dbContext.Products.FirstOrDefaultAsync(p => p.ProductId == orderLine.ProductId.Value);

                    if (product != null)
                    {
                        // Subtract the ordered quantity from the stock
                        product.Stoc -= orderLine.Quantity.Value;

                        if (product.Stoc < 0)
                        {
                            // Handle stock underflow, e.g., throw an exception or log a warning
                            throw new InvalidOperationException($"Stock for Product ID {product.ProductId} cannot be negative.");
                        }
                    }
                    else
                    {
                        // Handle case where product is not found
                        throw new KeyNotFoundException($"Product with ID {orderLine.ProductId.Value} not found.");
                    }
                }

                // Save changes to the database after updating all affected products
                await dbContext.SaveChangesAsync();
            }
            else
            {
                throw new InvalidOperationException("Order must be a PayedOrder to update stock.");
            }
        }
        
        /*public async Task<List<(int ProductId, decimal Price)>> GetPricesAsync(IEnumerable<string> productIdsToCheck)
        {
            var productIdsList = productIdsToCheck.ToList(); // Ensure it's a list

            // Query the database for matching product IDs
            var productsWithPrices = await dbContext.Products
                .AsNoTracking() // Optimize performance for read-only operations
                .Where(p => productIdsList.Contains(p.ProductId.ToString())) // Filter by provided product IDs
                .Select(p => new { p.ProductId, p.Price }) // Select only necessary fields
                .ToListAsync();

            // Return as a list of tuples (ProductId, Price)
            return productsWithPrices.Select(p => (p.ProductId, p.Price)).ToList();
        }*/





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
