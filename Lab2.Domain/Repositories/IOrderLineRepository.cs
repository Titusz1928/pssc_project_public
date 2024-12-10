using Lab2.Domain.Models;
using static Lab2.Domain.Models.Order;

namespace Lab2.Domain.Repositories
{
    public interface IOrderLineRepository
    {
        
        Task<List<CalculatedOrderLine>> GetExistingOrdersAsync();
        
        // Retrieve all order lines
        Task<List<PayedOrderLine>> GetAllOrderLinesAsync();

        // Retrieve a specific order line by its ID
        Task<PayedOrderLine> GetOrderLineByIdAsync(int orderLineId);

        // Add a new order line
        Task AddOrderLineAsync(IOrder orderLine);

        // Update an existing order line
        Task UpdateOrderLineAsync(PayedOrderLine orderLine);

        // Delete an order line by its ID
        Task DeleteOrderLineAsync(int orderLineId);
    }
}