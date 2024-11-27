using Lab2.Domain.Models;
using Lab2.Domain.Repositories;
using Microsoft.Extensions.Logging;
using Lab2.Domain.Operations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using static Lab2.Domain.Models.Order;
using static Lab2.Domain.Models.OrderCreatedEvent;

namespace Lab2.Domain.Workflows
{
    public class CreateOrderWorkflow
    {
        private readonly IProductsRepository productsRepository;
        private readonly IOrderLineRepository orderlineRepository;
        private readonly ILogger<CreateOrderWorkflow> logger;

        public CreateOrderWorkflow(IProductsRepository productsRepository, IOrderLineRepository orderlineRepository,
            ILogger<CreateOrderWorkflow> logger)
        {
            this.productsRepository = productsRepository;
            this.orderlineRepository = orderlineRepository;
            this.logger = logger;
        }

        public async Task<IOrderCreatedEvent> ExecuteAsync(CreateOrderCommand command)
        {
            try
            {
                //load state from database
                IEnumerable<string> productsToCheck = command.InputOrderLines.Select(product => product.ProductId);
                List<ProductId> existingProducts = await productsRepository.GetExistingProductsAsync(productsToCheck);
                List<CalculatedOrderLine> existingOrders = await orderlineRepository.GetExistingOrdersAsync();

                //business logic
                IOrder order = ExecuteBusinesslogic(command, existingProducts, existingOrders);

                if (order is Order.PayedOrder payedOrder)
                {
                    Console.WriteLine("saving order");
                    await orderlineRepository.AddOrderLineAsync(payedOrder);
                    await productsRepository.UpdateStockAsync(payedOrder);
                }



                return order.ToEvent();
            }
            catch (Exception ex)
            {
                logger.LogError(ex,"An error occured while creating the order");
                return new OrderCreatedEvent.OrderCreationFailedEvent("Unexpected error");
            }
        }

        private  Order.IOrder ExecuteBusinesslogic(
            CreateOrderCommand command,
            List<ProductId> existingProducts,
            List<CalculatedOrderLine> existingOrders)
        {
            
            Func<ProductId, bool> checkProductExists = code =>
            {
                bool exists = existingProducts.Any(s => s.Equals(code));
                return exists;
            };
            
            Func<ProductId, Task<Quantity>> GetAvailableStock = async productId =>
            {
                // Assuming the productId is being passed as a single ID, you can wrap it in an enumerable for the repository method
                var productIdsToCheck = new List<string> { productId.ToString() };
                return await productsRepository.GetAvailableStockAsync(productIdsToCheck);
            };

            /*Func<List<ProductId>, Task<Dictionary<ProductId, decimal>>> GetProductPrices = async productIds =>
            {
                // Fetch prices from the repository (assuming a method like GetPricesAsync exists)
                var prices = await productsRepository.GetPricesAsync(productIds);
                return prices.ToDictionary(p => new ProductId(p.ProductId), p => p.Price);
            };*/
            
            
            UnvalidatedOrder unvalidatedOrderLine = new(command.InputOrderLines);
            IOrder order = new ValidateOrderOperation(checkProductExists, GetAvailableStock).Transform(unvalidatedOrderLine);

            // Check if the order is valid
            if (order is Order.ValidatedOrder validatedOrder)
            {
                Console.WriteLine("Order validated \u2713");

                // Proceed to calculate the price
                order = new CalculateOrderOperation().Transform(order, existingOrders);
    
                // Check if the order is calculated
                if (order is Order.CalculatedOrder calculatedOrder)
                {
                    Console.WriteLine("Price calculated \u2713");

                    // Ask for payment confirmation
                    Console.WriteLine("Confirm payment (Y/N)");
                    string option = Console.ReadLine();

                    // If payment is confirmed, proceed to the payment step
                    if (option == "Y")
                    {
                        order = new PayOrderOperation().Transform(order);
            
                        // Check if the order has been paid
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