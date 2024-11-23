using Lab2.Domain.Models;
using Lab2.Domain.Repositories;
using Microsoft.Extensions.Logging;
using Lab2.Domain.Operations;

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
                IEnumerable<string> productsToCheck = command.InputOrderLines.Select(product => product.Code);
                List<Code> existingProducts = await productsRepository.GetExistingProductsAsync(productsToCheck);
                List<CalculatedOrderLine> existingOrders = await orderlineRepository.GetExistingOrdersAsync();

                //business logic
                IOrder order = ExecuteBusinesslogic(command, existingProducts, existingOrders);

                if (order is Order.PayedOrder payedOrder)
                {
                    Console.WriteLine("saving order");
                    await orderlineRepository.AddOrderLineAsync(payedOrder);
                }



                return order.ToEvent();
            }
            catch (Exception ex)
            {
                logger.LogError(ex,"An error occured while creating the order");
                return new OrderCreatedEvent.OrderCreationFailedEvent("Unexpected error");
            }
        }

        private static Order.IOrder ExecuteBusinesslogic(
            CreateOrderCommand command,
            List<Code> existingProducts,
            List<CalculatedOrderLine> existingOrders)
        {
            
            Func<Code, bool> checkProductExists = code =>
            {
                bool exists = existingProducts.Any(s => s.Equals(code));
                return exists;
            };
            UnvalidatedOrder unvalidatedOrderLine = new(command.InputOrderLines);
            
            IOrder order = new ValidateOrderOperation(checkProductExists).Transform(unvalidatedOrderLine);
            order = new CalculateOrderOperation().Transform(order,existingOrders);
            order = new PayOrderOperation().Transform(order);

            return order;
        }
    }
}