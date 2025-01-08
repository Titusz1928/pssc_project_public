using Lab2.Domain.Models;

using static Lab2.Domain.Models.Order;

namespace Lab2.Domain.Operations{

    internal sealed class ValidateOrderOperation : OrderOperation
    {
        private readonly Func<ProductId, bool> checkProductExists;
        private readonly Func<ProductId, Task<Quantity>> GetAvailableStock;
        
       internal ValidateOrderOperation(Func<ProductId, bool> checkProductExists, Func<ProductId, Task<Quantity>> getAvailableStock)
       {
           this.checkProductExists = checkProductExists;
           this.GetAvailableStock = getAvailableStock;
       }

       protected override IOrder OnUnvalidated(UnvalidatedOrder unvalidatedOrder)
       {
           // Validate the order lines
           (List<ValidatedOrderLine> validatedOrderLines, IEnumerable<string> orderLineErrors) = ValidateListOfOrderLines(unvalidatedOrder);

           // Validate the header
           (OrderHeader? validatedHeader, IEnumerable<string> headerErrors) = ValidateOrderHeader(unvalidatedOrder.Header);

           // Combine all errors
           var validationErrors = orderLineErrors.Concat(headerErrors).ToList();

           if (validationErrors.Any())
           {
               return new InvalidOrder(unvalidatedOrder.OrderList, validationErrors, unvalidatedOrder.Header);
           }
           else
           {
               return new ValidatedOrder(validatedOrderLines, validatedHeader!); // Header is guaranteed valid here
           }
       }
       
       private (OrderHeader?, IEnumerable<string>) ValidateOrderHeader(OrderHeader? header)
       {
           var errors = new List<string>();

           if (header == null)
           {
               errors.Add("Order header is missing.");
               return (null, errors);
           } 

           if (string.IsNullOrWhiteSpace(header.Name))
           {
               errors.Add("Name in the order header cannot be empty.");
           }

           if (string.IsNullOrWhiteSpace(header.Address))
           {
               errors.Add("Address in the order header cannot be empty.");
           }

           return errors.Any() ? (null, errors) : (header, errors);
       }

       private (List<ValidatedOrderLine>, IEnumerable<string>) ValidateListOfOrderLines(
           UnvalidatedOrder unvalidatedOrder)
       {
           List<String> validationErrors = [];
           List<ValidatedOrderLine> validatedOrderLines = [];

           foreach (UnvalidatedOrderLine unvalidatedOrderLine in unvalidatedOrder.OrderList)
           {
               ValidatedOrderLine? validOrderLine = ValidateOrderLine(unvalidatedOrderLine,validationErrors);
               if (validOrderLine is not null)
               {
                   validatedOrderLines.Add(validOrderLine);
               }
           }
           return (validatedOrderLines, validationErrors);
       }

       private ValidatedOrderLine? ValidateOrderLine(UnvalidatedOrderLine unvalidatedOrderLine,
           List<string> validationErrors)
       {
           List<string> currentValidationErrors = [];
           OrderId? orderid = ValidateAndParseOrderId(unvalidatedOrderLine,currentValidationErrors);
           ProductId? productId = ValidateAndParseProductId(unvalidatedOrderLine,currentValidationErrors);
           Quantity? quantity = ValidateAndParseQuantity(unvalidatedOrderLine,currentValidationErrors);
           Price? price = ValidateAndParsePrice(unvalidatedOrderLine,currentValidationErrors);

           ValidatedOrderLine? validOrderLine = null;
           if (!currentValidationErrors.Any())
           {
               validOrderLine = new(orderid,productId,quantity,price);
           }
           else
           {
               validationErrors.AddRange(currentValidationErrors);
           }
           return validOrderLine;
       }

       private OrderId? ValidateAndParseOrderId(UnvalidatedOrderLine unvalidatedOrderLine,
           List<string> validationErrors)
       {
           OrderId? orderId;
           if (!OrderId.TryParse(unvalidatedOrderLine.OrderId, out orderId))
           {
               validationErrors.Add($"Invalid order id: {unvalidatedOrderLine.OrderId}");
           }
           return orderId;
       }
       

       private ProductId? ValidateAndParseProductId(UnvalidatedOrderLine unvalidatedOrderLine,
           List<string> validationErrors)
       {
           ProductId? productId;
           if (!ProductId.TryParse(unvalidatedOrderLine.ProductId, out productId))
           {
               validationErrors.Add($"Invalid code: {unvalidatedOrderLine.ProductId}");
           }else if (!checkProductExists(productId!))
           {
               validationErrors.Add($"Product not found: {unvalidatedOrderLine.ProductId}");
           }
           return productId;
       }
       
       private Quantity? ValidateAndParseQuantity(UnvalidatedOrderLine unvalidatedOrderLine,
           List<string> validationErrors)
{
    Quantity? quantity;
    if (!Quantity.TryParse(unvalidatedOrderLine.Quantity, out quantity))
    {
        validationErrors.Add($"Invalid quantity: {unvalidatedOrderLine.ProductId}");
    }
    else
    {
        // Convert ProductId string to ProductId type if needed
        ProductId? productId = null;
        if (!ProductId.TryParse(unvalidatedOrderLine.ProductId, out productId))
        {
            validationErrors.Add($"Invalid product ID: {unvalidatedOrderLine.ProductId}");
        }

        if (productId != null)
        {
            // Get the available stock for the productId
            var availableStock = GetAvailableStock(productId!).Result; // Awaiting the result

            if (availableStock != null && availableStock.Value < quantity.Value) // Compare the underlying int values
            {
                validationErrors.Add($"Insufficient stock for product: {unvalidatedOrderLine.ProductId}");
            }
        }
    }

    return quantity;
}


       
       private Price? ValidateAndParsePrice(UnvalidatedOrderLine unvalidatedOrderLine,
           List<string> validationErrors)
       {
           Price? price;
           if (!Price.TryParsePrice(unvalidatedOrderLine.Price, out price))
           {
               validationErrors.Add($"Invalid quantity: {unvalidatedOrderLine.ProductId}");
           }

           return price;
       }
       
    }
}