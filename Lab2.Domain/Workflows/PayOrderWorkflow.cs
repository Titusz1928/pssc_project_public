using Lab2.Domain.Models;
using Lab2.Domain.Operations;
using Lab2.Domain.Repositories;
using Microsoft.Extensions.Logging;

//UNUSED WORKFLOW

namespace Lab2.Domain.Workflows
{

    public class PayOrderWorkflow
    {
        private readonly IProductsRepository productsRepository;
        private readonly IOrderLineRepository orderlineRepository;
        private readonly IOrderHeaderRepository orderHeaderRepository;
        private readonly ILogger<CreateOrderWorkflow> logger;

        public PayOrderWorkflow(IProductsRepository productsRepository, IOrderLineRepository orderlineRepository,
            IOrderHeaderRepository orderHeaderRepository, ILogger<CreateOrderWorkflow> logger)
        {
            this.productsRepository = productsRepository;
            this.orderlineRepository = orderlineRepository;
            this.orderHeaderRepository = orderHeaderRepository;
            this.logger = logger;
        }

        public async Task<OrderPaidEvent.IOrderPaidEvent> ExecuteAsync(PayOrderCommand command)
        {
            try
            {

                Order.IOrder order = ExecuteBusinessLogic(command);
                
                if (order is Order.PayedOrder payedOrder)
                {
                    Console.WriteLine("Saving order...");

                    // Save order header
                    await orderHeaderRepository.AddOrderHeaderAsync(payedOrder.Header);

                    Console.WriteLine($"orderid value:{payedOrder.Header.OrderId.Value}");
                    // Ensure OrderId is assigned if it's not already
                    if (payedOrder.Header.OrderId.Value == 0) // If OrderId is not assigned yet
                    {
                        payedOrder.Header.OrderId = await orderHeaderRepository.GetOrderIdForHeaderAsync(payedOrder.Header);
                    }

                    // Set OrderId on OrderLines using the `with` expression (immutability)
                    var updatedOrderList = new List<CalculatedOrderLine>();
                    foreach (var line in payedOrder.OrderList)
                    {
                        // Update the OrderId for each line and create a new updated line
                        var updatedLine = line with { OrderId = payedOrder.Header.OrderId };
                        updatedOrderList.Add(updatedLine);
                    }

                    // Create a new PayedOrder with the updated OrderList
                    var updatedPayedOrder = new Order.PayedOrder(
                        updatedOrderList, 
                        payedOrder.Header, 
                        payedOrder.Csv, 
                        payedOrder.CreatedDate
                    );
                
                    Console.WriteLine(payedOrder);
                    Console.WriteLine(updatedPayedOrder);

                    // Add order lines to repository (pass the updatedPayedOrder with updated lines)
                    await orderlineRepository.AddOrderLineAsync(updatedPayedOrder);  // Passing the new payedOrder with updated lines
                    await productsRepository.UpdateStockAsync(updatedPayedOrder);
                }

                return new OrderPaidEvent.OrderPaidFailedEvent("Unexpected order state");

            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while paying the order");
                return new OrderPaidEvent.OrderPaidFailedEvent("Unexpected error");
            }
        }

        private Order.IOrder ExecuteBusinessLogic(PayOrderCommand command)
        {
            // Assuming Order.CalculatedOrder has a constructor that takes an IReadOnlyCollection<CalculatedOrderLine>
            var order = new Order.CalculatedOrder(command.InputOrderLines, command.Header);

            if (order is Order.CalculatedOrder calculatedOrder)
            {
                // Step 3: Confirm payment
                return new PayOrderOperation().Transform(calculatedOrder);
            }
            else
            {
                logger.LogWarning("Order price calculation failed.");
                return order;
            }
        }
    }
}
    
    