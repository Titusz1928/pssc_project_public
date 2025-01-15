using System.Collections.ObjectModel;
using System.Net;
using System.Text;
using System.Text.Json;
using Lab2.API.Models;
using Lab2.Domain.Models;
using Lab2.Domain.Models.AwbContactInfo;
using Lab2.Domain.Repositories;
using Lab2.Domain.Workflows;
using Microsoft.AspNetCore.Mvc;
using Polly;

namespace Lab2.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class OrdersController : ControllerBase 
    {
        private readonly ILogger<OrdersController> _logger;
        private readonly CreateOrderWorkflow _createOrderWorkflow;
        private readonly PlaceOrderWorkflow _placeOrderWorkflow;

        public OrdersController(ILogger<OrdersController> logger, CreateOrderWorkflow createOrderWorkflow, PlaceOrderWorkflow placeOrderWorkflow)
        {
            _logger = logger;
            _createOrderWorkflow = createOrderWorkflow;
            _placeOrderWorkflow = placeOrderWorkflow;
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

            //CreateOrderCommand command = new(unvalidatedOrderLines, header);
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
        
        // Endpoint to place an order
[HttpPost("PlaceOrder")]
public async Task<IActionResult> PlaceOrder([FromBody] PlaceOrderRequest request)
{
    ReadOnlyCollection<UnvalidatedOrderLine> unvalidatedOrderLines = request.InputOrders
        .Select(MapInputOrderToUnvalidatedOrderLine)
        .ToList()
        .AsReadOnly();

    // Create header data using user input
    OrderHeader header = new OrderHeader(request.CustomerName, request.City, 0);

    // Create the PlaceOrderCommand
    PlaceOrderCommand command = new(unvalidatedOrderLines, header);

    // Execute the PlaceOrder workflow
    OrderPlacedEvent.IOrderPlacedEvent workflowResult = await _placeOrderWorkflow.ExecuteAsync(command);

    IActionResult response;

    // Using an explicit switch expression to handle different event types
    switch (workflowResult)
    {
        case OrderPlacedEvent.OrderPlacingSucceededEvent succeededEvent:
            {
                // Extract data from PayedOrder and send to Delivery API
                var payedOrderHeader = new OrderHeader(
                    succeededEvent.orderHeader.Name,  // Correct access to orderHeader property
                    succeededEvent.orderHeader.Address,
                    succeededEvent.orderHeader.OrderId
                );
                

                // Call the Delivery Workflow API
                var deliveryApiResult = await SendToDeliveryWorkflowAsync(payedOrderHeader, succeededEvent.Price, succeededEvent.CreatedDate);

                //if (deliveryApiResult.IsSuccessStatusCode)
                if(true)
                {
                    response = Ok(new { Csv = succeededEvent.Csv });
                }
                else
                {
                    // Retry logic can go here if needed
                    response = StatusCode((int)deliveryApiResult.StatusCode, "Failed to notify delivery workflow.");
                }
                break;
            }

        case OrderPlacedEvent.OrderPlacingFailedEvent failedEvent:
            response = BadRequest(failedEvent.Reasons);
            break;

        default:
            throw new NotImplementedException();
    }

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
        
        public class PlaceOrderRequest
        {
            public string CustomerName { get; set; }
            public string City { get; set; }
            public InputOrder[] InputOrders { get; set; }
        }
        
        private async Task<HttpResponseMessage> SendToDeliveryWorkflowAsync(OrderHeader orderHeader, Price price, DateTime createdDate)
        {
            using var client = new HttpClient();
            var deliveryApiUrl = "http://localhost:5246/api/AwbGenerator/generate";

            var phoneNr = "+734400184";
            // Ensure the phone number is directly passed as a plain string
            var unvalidatedAwbContactInfo = new UnvalidatedAwbContactInfo(
                "email@gmail.com",
                phoneNr  // No need for Unicode encoding, just use the + directly
            );

            var awbOrderInfo = new AwbOrderInfo(
                orderHeader,
                price,
                createdDate
            );

            // Create UnvalidatedAwb object to send in the request
            var unvalidatedAwb = new Awb.UnvalidatedAwb(unvalidatedAwbContactInfo, awbOrderInfo);

            // Serialize the UnvalidatedAwb object to JSON
            var jsonPayload = JsonSerializer.Serialize(new { UnvalidatedAwb = unvalidatedAwb });

            // Log the serialized JSON to the console
            Console.WriteLine("Serialized JSON Payload:");
            Console.WriteLine(jsonPayload);

            var content = new StringContent(
                jsonPayload,
                Encoding.UTF8,
                "application/json"
            );

            // Define the retry policy with exponential backoff
            var retryPolicy = Policy
                .Handle<HttpRequestException>()
                .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));

            // Execute the HTTP request with retry policy
            var response = await retryPolicy.ExecuteAsync(async () =>
            {
                var apiResponse = await client.PostAsync(deliveryApiUrl, content);
                if (apiResponse.IsSuccessStatusCode)
                {
                    return apiResponse;
                }

                // If the response is not successful, throw an exception to trigger the retry
                apiResponse.EnsureSuccessStatusCode();
                return apiResponse;
            });

            return response;
        }


        
    }
}
