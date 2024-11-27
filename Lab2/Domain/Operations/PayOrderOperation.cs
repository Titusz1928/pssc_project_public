using System.Text;
using System.Linq;
using Lab2.Domain.Models;
using static Lab2.Domain.Models.Order;

namespace Lab2.Domain.Operations{

    internal sealed class PayOrderOperation : OrderOperation
    {
        protected override IOrder OnCalculated(CalculatedOrder calculatedOrder)
        {
            StringBuilder csv = new();
            calculatedOrder.OrderList.Aggregate(csv, (export, order) =>
                export.AppendLine(GenerateCsvLine(order)));
            
            PayedOrder payedorder = new(calculatedOrder.OrderList, calculatedOrder.Header, csv.ToString(),DateTime.Now);
           return payedorder; 
        }

        private static string GenerateCsvLine(CalculatedOrderLine orderline) =>
            $"{orderline.ProductId.Value}, {orderline.Quantity.Value}, {orderline.Total.Value}";
    }

}