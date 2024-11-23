using Lab2.Domain.Models;

using static Lab2.Domain.Models.Order;

namespace Lab2.Domain.Operations{

    internal sealed class ValidateOrderOperation : OrderOperation
    {
        private readonly Func<Code, bool> checkProductExists;
        
       internal ValidateOrderOperation(Func<Code, bool> checkProductExists)
       {
           this.checkProductExists = checkProductExists;
       }

       protected override IOrder OnUnvalidated(UnvalidatedOrder unvalidatedOrder)
       {
           (List<ValidatedOrderLine> validatedOrderLines, IEnumerable<string> validationErrors)=ValidateListOfOrderLines(unvalidatedOrder);

           if (validationErrors.Any())
           {
               return new InvalidOrder(unvalidatedOrder.OrderList, validationErrors);
           }
           else
           {
               return new ValidatedOrder(validatedOrderLines);
           }
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
           Code? code = ValidateAndParseCode(unvalidatedOrderLine,currentValidationErrors);
           OrderId? orderid = ValidateAndParseOrderId(unvalidatedOrderLine,currentValidationErrors);
           ProductId? productId = ValidateAndParseProductId(unvalidatedOrderLine,currentValidationErrors);
           Quantity? quantity = ValidateAndParseQuantity(unvalidatedOrderLine,currentValidationErrors);
           Price? price = ValidateAndParsePrice(unvalidatedOrderLine,currentValidationErrors);

           ValidatedOrderLine? validOrderLine = null;
           if (!currentValidationErrors.Any())
           {
               validOrderLine = new(code,orderid,productId,quantity,price);
           }
           else
           {
               validationErrors.AddRange(currentValidationErrors);
           }
           return validOrderLine;
       }

       private Code? ValidateAndParseCode(UnvalidatedOrderLine unvalidatedOrderLine, List<string> validationErrors)
       {
           Code? code;
           if (!Code.TryParse(unvalidatedOrderLine.Code, out code))
           {
               validationErrors.Add($"Invalid code: {unvalidatedOrderLine.Code}");
           }else if (!checkProductExists(code!))
           {
               validationErrors.Add($"Product not found: {unvalidatedOrderLine.Code}");
           }
           return code;
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
               validationErrors.Add($"Invalid product id: {unvalidatedOrderLine.ProductId}");
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