using Lab2.Data.Models;
using Lab2.Domain.Models;
using Lab2.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Lab2.Data.Repositories
{
    public class OrderHeaderRepository : IOrderHeaderRepository
    {
        private readonly OrderLineContext dbContext;

        public OrderHeaderRepository(OrderLineContext dbContext)
        {
            this.dbContext = dbContext;
        }

        // This method adds a new OrderHeader and retrieves the generated OrderId
        public async Task AddOrderHeaderAsync(OrderHeader orderHeader)
        {
            if (orderHeader == null)
            {
                throw new ArgumentNullException(nameof(orderHeader), "OrderHeader cannot be null.");
            }

            // Create OrderHeaderDto from the provided OrderHeader
            var orderHeaderDto = new OrderHeaderDto
            {
                Name = orderHeader.Name,
                Address = orderHeader.Address
            };

            // Add the OrderHeaderDto to the DbContext
            dbContext.OrderHeaders.Add(orderHeaderDto);

            // Save changes to persist the new OrderHeaderDto and generate the OrderId
            await dbContext.SaveChangesAsync();

            // After saving, the OrderId will be automatically set on orderHeaderDto
            // Map the generated OrderId back to the orderHeader object
            orderHeader.OrderId = orderHeaderDto.OrderId;
        }

        // This method retrieves the OrderId for a specific OrderHeader
        public async Task<OrderId> GetOrderIdForHeaderAsync(OrderHeader orderHeader)
        {
            // Retrieve the OrderId for the specific OrderHeader from the database
            var orderHeaderDto = await dbContext.OrderHeaders
                .Where(o => o.Name == orderHeader.Name && o.Address == orderHeader.Address)
                .FirstOrDefaultAsync();

            if (orderHeaderDto == null)
            {
                throw new InvalidOperationException("OrderHeader not found.");
            }

            // Return the OrderId as an OrderId object (assuming OrderId is a class)
            return new OrderId(orderHeaderDto.OrderId);
        }
    }
}
