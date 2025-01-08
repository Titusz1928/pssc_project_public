using Lab2.Domain.Models;
using Lab2.Domain.Repositories;
using Microsoft.Extensions.Logging;
using Lab2.Domain.Operations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using static Lab2.Domain.Models.Order;
using static Lab2.Domain.Models.OrderCreatedEvent;


// FOR CONSOLE APP


namespace Lab2.Domain.Workflows
{
   public class CreateOrderWorkflow
{
    private readonly IProductsRepository productsRepository;
    private readonly IOrderLineRepository orderlineRepository;
    private readonly IOrderHeaderRepository orderHeaderRepository;
    private readonly ILogger<CreateOrderWorkflow> logger;

    public CreateOrderWorkflow(IProductsRepository productsRepository, IOrderLineRepository orderlineRepository, IOrderHeaderRepository orderHeaderRepository, ILogger<CreateOrderWorkflow> logger)
    {
        this.productsRepository = productsRepository;
        this.orderlineRepository = orderlineRepository;
        this.orderHeaderRepository = orderHeaderRepository;
        this.logger = logger;
    }

    public async Task<IOrderCreatedEvent> ExecuteAsync(CreateOrderCommand command)
    {
        try
        {
            // Load state from database
            IEnumerable<string> productsToCheck = command.InputOrderLines.Select(product => product.ProductId);
            List<ProductId> existingProducts = await productsRepository.GetExistingProductsAsync(productsToCheck);
            List<CalculatedOrderLine> existingOrders = await orderlineRepository.GetExistingOrdersAsync();

            // Business logic execution
            IOrder order = ExecuteBusinessLogic(command, existingProducts, existingOrders);

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

            return OrderCreatedEvent.ToEvent(order);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while creating the order");
            return new OrderCreatedEvent.OrderCreationFailedEvent("Unexpected error");
        }
    }

    private IOrder ExecuteBusinessLogic(CreateOrderCommand command, List<ProductId> existingProducts, List<CalculatedOrderLine> existingOrders)
    {
        Func<ProductId, bool> checkProductExists = code => existingProducts.Any(s => s.Equals(code));
        Func<ProductId, Task<Quantity>> GetAvailableStock = async productId =>
        {
            var productIdsToCheck = new List<string> { productId.ToString() };
            return await productsRepository.GetAvailableStockAsync(productIdsToCheck);
        };

        UnvalidatedOrder unvalidatedOrderLine = new(command.InputOrderLines, (OrderHeader?)command.Header);
        IOrder order = new ValidateOrderOperation(checkProductExists, GetAvailableStock).Transform(unvalidatedOrderLine);

        if (order is Order.ValidatedOrder validatedOrder)
        {
            Console.WriteLine("Order validated \u2713");

            // Proceed to calculate the price
            order = new CalculateOrderOperation().Transform(order, existingOrders);

            if (order is Order.CalculatedOrder calculatedOrder)
            {
                Console.WriteLine("Price calculated \u2713");

                // Confirm payment
                Console.WriteLine("Confirm payment (Y/N)");
                string option = Console.ReadLine();

                if (option == "Y")
                {
                    order = new PayOrderOperation().Transform(order);

                    if (order is Order.PayedOrder payedOrder)
                    {
                        Console.WriteLine("Payment completed \u2713");
                    }
                    else
                    {
                        Console.WriteLine("Payment failed");
                    }
                }
                else
                {
                    Console.WriteLine("Payment not confirmed. Order not completed.");
                }
            }
            else
            {
                Console.WriteLine("Order price calculation failed.");
            }
        }
        else
        {
            Console.WriteLine("Order validation failed.");
        }

        return order;
    }
}

}