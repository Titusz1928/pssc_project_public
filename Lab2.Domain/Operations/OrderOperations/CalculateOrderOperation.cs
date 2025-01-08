using Lab2.Domain.Models;
using System.Collections.Generic;
using System.Linq;
using static Lab2.Domain.Models.Order;

namespace Lab2.Domain.Operations
{
    internal sealed class CalculateOrderOperation : OrderOperation<List<CalculatedOrderLine>>
    {
        internal CalculateOrderOperation()
        {
        }

        protected override IOrder OnValid(ValidatedOrder validOrderLine, List<CalculatedOrderLine>? existingOrderLines) =>
            new CalculatedOrder(
                validOrderLine.OrderList
                    .Select(validOrder => CalculateAndMatchOrderLine(existingOrderLines, validOrder))
                    .ToList()
                    .AsReadOnly()
            ,validOrderLine.Header
                );

        private static CalculatedOrderLine CalculateAndMatchOrderLine(List<CalculatedOrderLine>? existingOrderLines, ValidatedOrderLine validOrderLine)
        {
            // Find an existing order line that matches by ProductId
            var existingOrderLine = existingOrderLines?.FirstOrDefault(
                line => line.ProductId.ToString() == validOrderLine.ProductId.ToString());

            // Calculate total price for the order line
            var total = CalculateTotal(validOrderLine);

            // Create a new CalculatedOrderLine
            return new CalculatedOrderLine(
                validOrderLine.OrderId,
                validOrderLine.ProductId,
                validOrderLine.Quantity,
                total
            );
        }

        private static Total? CalculateTotal(ValidatedOrderLine validOrderLine)
        {
            if (validOrderLine.Quantity != null && validOrderLine.Price != null)
            {
                // Multiply Quantity.Value and Price.Value
                var totalValue = validOrderLine.Quantity.Value * (float)validOrderLine.Price.Value;
                return new Total(totalValue);
            }

            return null;
        }
    }
}
