using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lab2.Data.Models;
using Lab2.Domain.Models;
using Lab2.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Lab2.Data.Repositories
{
    public class OrderLineRepository : IOrderLineRepository
    {
        private readonly OrderLineContext dbContext;

        public OrderLineRepository(OrderLineContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<List<CalculatedOrderLine>> GetExistingOrdersAsync()
        {
            var foundProductOrders = await
                (
                    from o in dbContext.Orders
                    join p in dbContext.Products on o.ProductId equals p.ProductId
                    select new { p.Code, o.OrderId, o.ProductId, o.Quantity, o.Price, o.Total }
                ).AsNoTracking()
                .ToListAsync();

            List<CalculatedOrderLine> foundOrdersModel = foundProductOrders.Select(result =>
                new CalculatedOrderLine(
                    OrderId: new OrderId(result.OrderId),
                    ProductId: new ProductId(result.ProductId),
                    Quantity: new Quantity(result.Quantity),
                    Price: new Price(result.Price),
                    Total: new Total(result.Total)
                )).ToList();

            return foundOrdersModel;
        }

        // Retrieve all PayedOrders with their associated product details
        public async Task<List<PayedOrderLine>> GetAllOrderLinesAsync()
        {
            var orderLines = await (
                from o in dbContext.Orders
                join p in dbContext.Products on o.ProductId equals p.ProductId
                select new
                {
                    o.OrderLineId,
                    p.Code,
                    o.ProductId,
                    o.Quantity,
                    o.Price,
                    o.OrderId,
                    o.Total
                }
            ).AsNoTracking().ToListAsync();

            return orderLines.Select(o => new PayedOrderLine(
                OrderId: new OrderId(o.OrderId),
                ProductId: new ProductId(o.ProductId),
                Quantity: new Quantity(o.Quantity),
                Price: new Price(o.Price),
                Total: new Total(o.Total))
            {
                OrderLineId = o.OrderLineId,
                IsUpdated = false // Default to false unless updated elsewhere
            }).ToList();
        }

        // Retrieve a PayedOrder by its ID
        public async Task<PayedOrderLine?> GetOrderLineByIdAsync(int orderLineId)
        {
            var orderLine = await (
                from o in dbContext.Orders
                join p in dbContext.Products on o.ProductId equals p.ProductId
                where o.OrderLineId == orderLineId
                select new
                {
                    o.OrderLineId,
                    p.Code,
                    o.ProductId,
                    o.Quantity,
                    o.Price,
                    o.OrderId,
                    o.Total
                }
            ).AsNoTracking().FirstOrDefaultAsync();

            if (orderLine == null) return null;

            return new PayedOrderLine(
                OrderId: new OrderId(orderLine.OrderId),
                ProductId: new ProductId(orderLine.ProductId),
                Quantity: new Quantity(orderLine.Quantity),
                Price: new Price(orderLine.Price),
                Total: new Total(orderLine.Total))
            {
                OrderLineId = orderLine.OrderLineId,
                IsUpdated = false
            };
        }
        
        public async Task AddOrderLineAsync(Order.IOrder order)
        {
            if (order is Order.PayedOrder payedOrder)
            {
                foreach (var orderLine in payedOrder.OrderList)
                {
                    var orderLineDto = new OrderLineDto
                    {
                        ProductId = orderLine.ProductId?.Value ?? 0, // Handle nullable values
                        Quantity = orderLine.Quantity?.Value ?? 0,  // Handle nullable values
                        Price = orderLine.Price?.Value ?? 0f,       // Handle nullable values
                        Total = orderLine.Total?.Value ?? 0f,       // Handle nullable values
                        OrderId = orderLine.OrderId?.Value ?? 0     // Handle nullable values
                    };

                    dbContext.Orders.Add(orderLineDto); // Ensure the DbSet is named "OrderLines"
                }

                await dbContext.SaveChangesAsync();
            }
            else
            {
                throw new InvalidOperationException("The provided order is not a valid PayedOrder.");
            }
        }


        // Update an existing PayedOrder
        public async Task UpdateOrderLineAsync(PayedOrderLine orderLine)
        {
            var orderLineDto = new OrderLineDto
            {
                OrderLineId = orderLine.OrderLineId,
                ProductId = orderLine.ProductId.Value,
                Quantity = orderLine.Quantity.Value,
                Price = orderLine.Price.Value,
                Total = orderLine.Total.Value,
                OrderId = orderLine.OrderId.Value
            };

            dbContext.Entry(orderLineDto).State = EntityState.Modified;
            await dbContext.SaveChangesAsync();
        }

        // Delete a PayedOrder by ID
        public async Task DeleteOrderLineAsync(int orderLineId)
        {
            var orderLineDto = await dbContext.Orders.FindAsync(orderLineId);
            if (orderLineDto != null)
            {
                dbContext.Orders.Remove(orderLineDto);
                await dbContext.SaveChangesAsync();
            }
        }
        
    }
}
