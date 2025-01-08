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

        internal OrderPlacingSucceededEvent(OrderHeader _orderHeader, string csv, DateTime createdDate)
        {
            orderHeader = _orderHeader;
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

    public static IOrderPlacedEvent ToEvent(this Order.IOrder order) =>
        order switch
        {
            Order.UnvalidatedOrder _ => new OrderPlacingFailedEvent("unexpected unvalidated state"),
            Order.ValidatedOrder validatedOrders => new OrderPlacingFailedEvent("unexpected validated state"),
            Order.CalculatedOrder calculatedOrders => new OrderPlacingFailedEvent("unexpected calculated state"),
            Order.InvalidOrder invalidOrders => new OrderPlacingFailedEvent(invalidOrders.Reasons),
            Order.PayedOrder payedOrders => new OrderPlacingSucceededEvent(payedOrders.Header, payedOrders.Csv, payedOrders.CreatedDate),
            _ => throw new NotImplementedException(),
        };
}