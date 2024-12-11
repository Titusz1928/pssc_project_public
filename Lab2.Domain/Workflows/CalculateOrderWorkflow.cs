using Lab2.Domain.Models;
using Lab2.Domain.Repositories;
using Microsoft.Extensions.Logging;
using Lab2.Domain.Operations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using static Lab2.Domain.Models.Order;
using static Lab2.Domain.Models.OrderCreatedEvent;


namespace Lab2.Domain.Workflows
{

    public class CalculateOrderWorkflow
    { 
        private readonly IProductsRepository productsRepository;
    private readonly IOrderLineRepository orderlineRepository;
    private readonly IOrderHeaderRepository orderHeaderRepository;
    private readonly ILogger<CreateOrderWorkflow> logger;

    public CalculateOrderWorkflow(IProductsRepository productsRepository, IOrderLineRepository orderlineRepository, IOrderHeaderRepository orderHeaderRepository, ILogger<CreateOrderWorkflow> logger)
    {
        this.productsRepository = productsRepository;
        this.orderlineRepository = orderlineRepository;
        this.orderHeaderRepository = orderHeaderRepository;
        this.logger = logger;
    }

    public async Task<IOrder> ExecuteAsync(CalculateOrderCommand command)
    {
        try
        {
            // Load state from the database
            IEnumerable<string> productsToCheck = command.InputOrderLines.Select(product => product.ProductId);
            List<ProductId> existingProducts = await productsRepository.GetExistingProductsAsync(productsToCheck);
            List<CalculatedOrderLine> existingOrders = await orderlineRepository.GetExistingOrdersAsync();

            // Business logic execution
            IOrder order = ExecuteBusinessLogic(command, existingProducts, existingOrders);

            if (order is Order.CalculatedOrder calculatedOrder)
            {
                Console.WriteLine("Order calculated successfully!");
                //VisualizeOrder(calculatedOrder); // Display the calculated order for the user
                return calculatedOrder; // Pass the order for the next workflow
            }
            else
            {
                Console.WriteLine("Order calculation failed.");
                return new Order.InvalidOrder(
                    command.InputOrderLines, 
                    new List<string> { "Order calculation failed due to validation or business logic issues." }, 
                    command.Header
                );
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while calculating the order");
            return new Order.InvalidOrder(
                command.InputOrderLines, 
                new List<string> { "Unexpected error occurred during order calculation." }, 
                command.Header
            );
        }
    }


    private IOrder ExecuteBusinessLogic(CalculateOrderCommand command, List<ProductId> existingProducts, List<CalculatedOrderLine> existingOrders)
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

                //ready for payorderworkflow
                return order;

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