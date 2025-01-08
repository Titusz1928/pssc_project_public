using Lab2.Domain.Models;
using Lab2.Domain.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using Lab2.Domain.Workflows;

namespace Lab2.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProductsController : ControllerBase
    {
        private static readonly List<UnvalidatedOrderLine> UnvalidatedUserOrders = new();
        private static readonly List<CalculatedOrderLine> CalculatedUserOrders = new();
        
        private readonly CalculateOrderWorkflow _calculateOrderWorkflow;
        private readonly PayOrderWorkflow _payOrderWorkflow;

        private readonly ILogger<ProductsController> _logger;
        private readonly IProductsRepository _productsRepository;

        public ProductsController(
            ILogger<ProductsController> logger,
            IProductsRepository productsRepository,
            CalculateOrderWorkflow calculateOrderWorkflow,
            PayOrderWorkflow payOrderWorkflow)
        {
            _logger = logger;
            _productsRepository = productsRepository;
            _calculateOrderWorkflow = calculateOrderWorkflow;
            _payOrderWorkflow=payOrderWorkflow;
        }
        
        
        // Endpoint to retrieve all products
        [HttpGet("GetAllProducts")]
public async Task<IActionResult> GetAllProducts()
{
    var products = await _productsRepository.GetAllProductsAsync();

    // Retrieve the user's order if it exists
    var orderLines = CalculatedUserOrders;

    // Generate HTML page with interactive elements
    var htmlBuilder = new StringBuilder();
    htmlBuilder.Append("<html><head><title>Products</title>");
htmlBuilder.Append("<script>");
htmlBuilder.Append(@"
    // Function to open the modal for a specific product
    function openModal(productId, productCode) {
        const modal = document.getElementById('productModal');
        modal.style.display = 'block';
        document.getElementById('modalProductId').innerText = productId;
        document.getElementById('modalProductCode').innerText = productCode;
    }

    // Function to close the modal
    function closeModal() {
        const modal = document.getElementById('productModal');
        modal.style.display = 'none';
    }

    // Function to confirm the selection
    function confirmSelection() {
        const quantity = document.getElementById('quantityInput').value;
        const productId = document.getElementById('modalProductId').innerText;

        // Prepare the request data
        const requestData = {
            UserId: 'user123',  // Example user ID (this should be dynamic or set on login)
            ProductId: productId,
            Quantity: quantity
        };

        // Send the data to the AddToOrder endpoint
        fetch('/Products/AddToOrder', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(requestData)
        })
        .then(response => response.json())
        .then(data => {
            // Handle the response and update the HTML
            alert('Product added to order! Total Price: ' + data.TotalPrice);
            closeModal();

            // Update the order details on the page
            console.log(data);
            updateOrderDetails(data.orderLines, data.TotalPrice);
        })
        .catch(error => {
            console.error('Error:', error);
            alert('There was an error adding the product to the order.');
            closeModal();
        });
    }

    // Function to update the order details in the HTML
    function updateOrderDetails(orderLines, totalPrice) {
        const orderTable = document.getElementById('orderTable');  // Assume there's an element with id 'orderTable'
        const totalPriceElement = document.getElementById('totalPrice');  // Assume there's an element with id 'totalPrice'

        // Clear the current order table rows (except the header)
        orderTable.innerHTML = '<tr><th>Product ID</th><th>Quantity</th><th>Total</th></tr>';

        console.log(orderLines);

        // Loop through the order lines and add them to the table
        orderLines.forEach(line => {
            const row = orderTable.insertRow();
            row.insertCell(0).innerText = line.productId.value;
            console.log(line.productId);
            row.insertCell(1).innerText = line.quantity.value;
            console.log(line.quantity);
            row.insertCell(2).innerText = line.total.value;
            console.log(line.total);
        });
    }

    // Add event listener for the Pay button
    document.addEventListener('DOMContentLoaded', function() {
        document.getElementById('payOrderButton').addEventListener('click', function() {
            // Send a POST request to the PayOrder endpoint
            fetch('/Products/PayOrder', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
            })
            .then(response => response.json())
            .then(data => {
                // Handle the response from the server
                console.log('Pay Order Success:', data);
                alert('Order has been paid successfully!');
            })
            .catch(error => {
                // Handle errors
                console.error('Error paying order:', error);
                alert('There was an error processing the payment.');
            });
        });
    });
");
htmlBuilder.Append("</script>");
htmlBuilder.Append("<style>");
htmlBuilder.Append(@"
    #productModal {
        display: none;
        position: fixed;
        top: 50%;
        left: 50%;
        transform: translate(-50%, -50%);
        background-color: white;
        border: 1px solid black;
        padding: 20px;
    }
");
htmlBuilder.Append("</style>");
htmlBuilder.Append("</head><body>");
htmlBuilder.Append("<h1>Product List</h1>");
htmlBuilder.Append("<table border='1'>");
htmlBuilder.Append("<tr><th>Product ID</th><th>Code</th><th>Stock</th><th>Actions</th></tr>");

// Loop through products and generate table rows
foreach (var product in products)
{
    htmlBuilder.Append($"<tr>");
    htmlBuilder.Append($"<td>{product.ProductId}</td>");
    htmlBuilder.Append($"<td>{product.Code.Value}</td>");
    htmlBuilder.Append($"<td>{product.Stoc}</td>");
    htmlBuilder.Append($"<td><button onclick=\"openModal('{product.ProductId}', '{product.Code.Value}')\">Select Quantity</button></td>");
    htmlBuilder.Append($"</tr>");
}
htmlBuilder.Append("</table>");

// Modal for selecting quantity
htmlBuilder.Append(@"
<div id='productModal'>
    <h2>Product Selection</h2>
    <p>Product ID: <span id='modalProductId'></span></p>
    <p>Product Code: <span id='modalProductCode'></span></p>
    <label for='quantityInput'>Quantity:</label>
    <input type='number' id='quantityInput' min='1' />
    <br><br>
    <button onclick='confirmSelection()'>Confirm</button>
    <button onclick='closeModal()'>Close</button>
</div>
");

// Display the order details (if available)
htmlBuilder.Append("<h2>Your Order</h2>");
htmlBuilder.Append("<table border='1' id=\"orderTable\">");
htmlBuilder.Append("<tr><th>Product ID</th><th>Quantity</th><th>Total</th></tr>");

// If orderLines is not empty, append rows for each line
if (orderLines.Any())
{
    foreach (var line in orderLines)
    {
        htmlBuilder.Append($"<tr>");
        htmlBuilder.Append($"<td>{line.ProductId}</td>");
        htmlBuilder.Append($"<td>{line.Quantity}</td>");
        htmlBuilder.Append($"<td>{line.Total.Value.ToString("C")}</td>");  // Display the 'Total' as a currency-formatted float
        htmlBuilder.Append($"</tr>");
    }

    // Calculate the total price
    var totalPrice = orderLines.Sum(line => line.Total.Value); // Sum the 'Value' of each 'Total'
    htmlBuilder.Append($"<p><strong>Total Price: {totalPrice.ToString("C")}</strong></p>");  // Display the total price as currency
}
else
{
    htmlBuilder.Append("<tr><td colspan='3'>No items in the cart yet.</td></tr>");
}

htmlBuilder.Append("</table>");

// Pay button
htmlBuilder.Append("<button id='payOrderButton'>Pay</button>");

htmlBuilder.Append("</body></html>");

return Content(htmlBuilder.ToString(), "text/html");

}
        
        


        [HttpPost("AddToOrder")]
public async Task<IActionResult> AddToOrder([FromBody] AddToOrderRequest request)
{
    // Add the product to UnvalidatedUserOrders
    var existingOrderLine = UnvalidatedUserOrders.FirstOrDefault(line => line.ProductId == request.ProductId);

    if (existingOrderLine != null)
    {
        // Remove the old order line
        UnvalidatedUserOrders.Remove(existingOrderLine);

        // Create a new UnvalidatedOrderLine with the combined quantity
        var newOrderLine = new UnvalidatedOrderLine(
            "0",  // Default "0" for OrderId
            request.ProductId,
            (int.Parse(existingOrderLine.Quantity) + request.Quantity).ToString(),
            ProductPrices[int.Parse(request.ProductId)].ToString()
        );

        // Add the new order line with the updated quantity
        UnvalidatedUserOrders.Add(newOrderLine);
    }
    else
    {
        // Add a new order line if no existing order line is found
        var newOrderLine = new UnvalidatedOrderLine(
            "0",  // Default "0" for OrderId
            request.ProductId,
            request.Quantity.ToString(),
            ProductPrices[int.Parse(request.ProductId)].ToString()
        );

        UnvalidatedUserOrders.Add(newOrderLine);
    }

    // Create an unvalidated order with the header
    var orderHeader = new OrderHeader("John Doe", "1234 Main St", 0); // Placeholder address and user name

    // Create the CalculateOrderCommand with the unvalidated order
    CalculateOrderCommand command = new(UnvalidatedUserOrders, orderHeader);

    // Execute the CalculateOrderWorkflow asynchronously
    Order.IOrder workflowResult = await _calculateOrderWorkflow.ExecuteAsync(command);

    // Check if the result is a CalculatedOrder
    if (workflowResult is Order.CalculatedOrder calculatedOrder)
    {
        _logger.LogInformation("Calculated Order: " + calculatedOrder);

        // Map CalculatedOrderLine to UnvalidatedOrderLine
        var unvalidatedOrderLines = calculatedOrder.OrderList.Select(line => new UnvalidatedOrderLine(
            "0",  // Default "0" for OrderId
            line.ProductId.ToString(),
            line.Quantity.ToString(),
            ProductPrices[line.ProductId.Value].ToString()
        )).ToList();

        // Now, move the calculated order to CalculatedUserOrders
        CalculatedUserOrders.Clear();
        CalculatedUserOrders.AddRange(calculatedOrder.OrderList.Select(line => new CalculatedOrderLine(
            0,
            line.ProductId,
            line.Quantity,
            line.Total
        )));

        // Return the response with updated order lines
        var calculatedOrderLines = calculatedOrder.OrderList.Select(line => new
        {
            ProductId = line.ProductId,
            Quantity = line.Quantity,
            Total = line.Total
        }).ToList();

        // Calculate the total price of the order
        var totalPrice = calculatedOrder.OrderList.Sum(line => line.Total != null ? (decimal)line.Total.Value : 0m);

        return Ok(new
        {
            OrderHeader = calculatedOrder.Header,
            OrderLines = calculatedOrderLines,
            TotalPrice = totalPrice
        });
    }
    else
    {
        // If the result is not a CalculatedOrder, handle the error appropriately
        return BadRequest("Order calculation failed.");
    }
}




        public class AddToOrderRequest
        {
            public string UserId { get; set; } // Unique identifier for the user
            public string ProductId { get; set; }
            public int Quantity { get; set; }
        }
        
        private static readonly Dictionary<int, float> ProductPrices = new()
        {
            { 1, 10.5f }, // ProductId 1 -> Price 10.5
            { 2, 20.0f }  // ProductId 2 -> Price 20.0
        };


        [HttpPost("PayOrder")]
        public async Task<IActionResult> PayOrder()
        {
            _logger.LogInformation("Pay Order");

            // Placeholder order header
            var orderHeader = new OrderHeader("John Doe", "1234 Main St", 0);
    
            // Create command object
            PayOrderCommand command = new(CalculatedUserOrders, orderHeader);

            // Execute workflow and get event
            var workflowResult = await _payOrderWorkflow.ExecuteAsync(command);

            // Ensure the result is converted into a response event
            if (workflowResult is OrderPaidEvent.IOrderPaidEvent paidEvent)
            {
                return Ok(paidEvent); // Return the event if it was successful
            }
    
            // If an error or unexpected result
            return BadRequest("Failed to process the order");
        }

    }
}
