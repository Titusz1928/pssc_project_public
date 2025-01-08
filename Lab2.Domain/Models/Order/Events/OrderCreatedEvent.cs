using System;
using System.Collections.Generic;

namespace Lab2.Domain.Models;

public static  class OrderCreatedEvent
{
    public interface IOrderCreatedEvent
    {
        
    }

    public record OrderCreationSucceededEvent : IOrderCreatedEvent
    {
        public string Csv { get; }
        public DateTime CreatedDate { get; }

        internal OrderCreationSucceededEvent(string csv, DateTime createdDate)
        {
            Csv = csv;
            CreatedDate = createdDate;
        }
    }

    public record OrderCreationFailedEvent : IOrderCreatedEvent
    {
        public IEnumerable<string> Reasons { get; }

        internal OrderCreationFailedEvent(string reasons)
        {
            Reasons = [reasons];
        }
        
        internal OrderCreationFailedEvent(IEnumerable<string> reasons)
        {
            Reasons = reasons;
        }
    }

    public static IOrderCreatedEvent ToEvent(this Order.IOrder order) =>
        order switch
        {
            Order.UnvalidatedOrder _ => new OrderCreationFailedEvent("unexpected unvalidated state"),
            Order.ValidatedOrder validatedOrders => new OrderCreationFailedEvent("unexpected validated state"),
            Order.CalculatedOrder calculatedOrders => new OrderCreationFailedEvent("unexpected calculated state"),
            Order.InvalidOrder invalidOrders => new OrderCreationFailedEvent(invalidOrders.Reasons),
            Order.PayedOrder payedOrders => new OrderCreationSucceededEvent(payedOrders.Csv, payedOrders.CreatedDate),
            _ => throw new NotImplementedException(),
        };
}