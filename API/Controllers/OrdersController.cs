using System.Collections.ObjectModel;
using Lab2.API.Models;
using Lab2.Domain.Models;
using Lab2.Domain.Repositories;
using Lab2.Domain.Workflows;
using Microsoft.AspNetCore.Mvc;

namespace Lab2.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly ILogger<OrdersController> _logger;
        private readonly CreateOrderWorkflow _createOrderWorkflow;

        public OrdersController(ILogger<OrdersController> logger, CreateOrderWorkflow createOrderWorkflow)
        {
            _logger = logger;
            _createOrderWorkflow = createOrderWorkflow;
        }

        // Endpoint to retrieve all orders
        [HttpGet("GetAllOrders")]
        public async Task<IActionResult> GetAllOrders([FromServices] IOrderLineRepository orderLineRepository)
        {
            List<PayedOrderLine> orders = await orderLineRepository.GetAllOrderLinesAsync();
            var result = orders.Select(order => new
            {
                OrderId = order.OrderId.Value,
                ProductId = order.ProductId.Value,
                Quantity = order.Quantity.Value,
                Total = order.Total.Value
            });
            return Ok(result);
        }

        // Endpoint to create an order
        [HttpPost("CreateOrder")]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequest request)
        {
            ReadOnlyCollection<UnvalidatedOrderLine> unvalidatedOrderLines = request.InputOrders
                .Select(MapInputOrderToUnvalidatedOrderLine)
                .ToList()
                .AsReadOnly();

            // Create header data using user input
            OrderHeader header = new OrderHeader(request.CustomerName, request.City, 0);

            CreateOrderCommand command = new(unvalidatedOrderLines, header);
            OrderCreatedEvent.IOrderCreatedEvent workflowResult = await _createOrderWorkflow.ExecuteAsync(command);

            IActionResult response = workflowResult switch
            {
                OrderCreatedEvent.OrderCreationSucceededEvent @event => Ok(new { Csv = @event.Csv }),
                OrderCreatedEvent.OrderCreationFailedEvent @event => BadRequest(@event.Reasons),
                _ => throw new NotImplementedException()
            };

            return response;
        }

        // Helper method to map InputOrder to UnvalidatedOrderLine
        private static UnvalidatedOrderLine MapInputOrderToUnvalidatedOrderLine(InputOrder order)
        {
            // Here we use "0" as a placeholder for OrderId because it's unvalidated
            return new UnvalidatedOrderLine("0", order.ProductId.ToString(), order.Quantity.ToString(), ProductPrices[order.ProductId].ToString());
        }

        // Example Product Prices dictionary (can be expanded as needed)
        private static readonly Dictionary<int, float> ProductPrices = new()
        {
            { 1, 10.5f }, // ProductId 1 -> Price 10.5
            { 2, 20.0f }  // ProductId 2 -> Price 20.0
        };
        
        public class CreateOrderRequest
        {
            public string CustomerName { get; set; }
            public string City { get; set; }
            public InputOrder[] InputOrders { get; set; }
        }
    }
}
