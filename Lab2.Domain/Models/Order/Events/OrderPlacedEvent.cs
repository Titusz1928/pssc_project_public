using System;
using System.Collections.Generic;

namespace Lab2.Domain.Models;

public static  class OrderPlacedEvent
{
    public interface IOrderPlacedEvent
    {
        
    }

    public record OrderPlacingSucceededEvent : IOrderPlacedEvent
    {
        public OrderHeader orderHeader { get; }
        public string Csv { get; }
        public DateTime CreatedDate { get; }
        
        public Price Price { get; }

        internal OrderPlacingSucceededEvent(OrderHeader _orderHeader, string csv, DateTime createdDate, Price price)
        {
            orderHeader = _orderHeader;
            Price = price;
            Csv = csv;
            CreatedDate = createdDate;
        }
    }

    public record OrderPlacingFailedEvent : IOrderPlacedEvent
    {
        public IEnumerable<string> Reasons { get; }

        internal OrderPlacingFailedEvent(string reasons)
        {
            Reasons = [reasons];
        }
        
        internal OrderPlacingFailedEvent(IEnumerable<string> reasons)
        {
            Reasons = reasons;
        }
    }

    public static IOrderPlacedEvent ToEvent(this Order.IOrder order)
    {
        switch (order)
        {
            case Order.UnvalidatedOrder _:
                return new OrderPlacingFailedEvent("unexpected unvalidated state");

            case Order.ValidatedOrder validatedOrders:
                return new OrderPlacingFailedEvent("unexpected validated state");

            case Order.CalculatedOrder calculatedOrders:
                return new OrderPlacingFailedEvent("unexpected calculated state");

            case Order.InvalidOrder invalidOrders:
                return new OrderPlacingFailedEvent(invalidOrders.Reasons);

            case Order.PayedOrder payedOrders:
                // Calculate the total by summing up the "Total" values from each CalculatedOrderLine in the OrderList
                Price total = CalculateTotal(payedOrders);

                // Create and return the OrderPlacingSucceededEvent with the calculated total
                return new OrderPlacingSucceededEvent(payedOrders.Header, payedOrders.Csv, payedOrders.CreatedDate, total);

            default:
                throw new NotImplementedException();
        }
    }


    private static Price CalculateTotal(Order.PayedOrder payedOrder)
    {
        // Sum the "Total" values from each CalculatedOrderLine in the OrderList
        float sum = 0;
        foreach (var orderLine in payedOrder.OrderList)
        {
            sum += orderLine.Total?.Value ?? 0;  // Use Value if Total is not null, otherwise use 0
        }

        // Return the total as a Price object
        return new Price(sum);
    }

}