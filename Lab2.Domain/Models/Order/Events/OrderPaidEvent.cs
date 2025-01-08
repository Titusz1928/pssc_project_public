using System;
using System.Collections.Generic;

namespace Lab2.Domain.Models
{
    public static class OrderPaidEvent
    {
        public interface IOrderPaidEvent
        {
        }

        public record OrderPaidSucceededEvent : IOrderPaidEvent
        {
            public string Csv { get; }
            public DateTime CreatedDate { get; }

            internal OrderPaidSucceededEvent(string csv, DateTime createdDate)
            {
                Csv = csv;
                CreatedDate = createdDate;
            }
        }

        public record OrderPaidFailedEvent : IOrderPaidEvent
        {
            public IEnumerable<string> Reasons { get; }

            internal OrderPaidFailedEvent(string reason)
            {
                Reasons = new List<string> { reason };
            }

            internal OrderPaidFailedEvent(IEnumerable<string> reasons)
            {
                Reasons = reasons;
            }
        }

        // Method to convert a PayOrderWorkflow result (IOrder) to an event
        public static IOrderPaidEvent ToEvent(this Order.IOrder order) =>
            order switch
            {
                Order.PayedOrder payedOrder => new OrderPaidSucceededEvent(payedOrder.Csv, payedOrder.CreatedDate),
                Order.InvalidOrder failedOrder => new OrderPaidFailedEvent(failedOrder.Reasons),
                _ => throw new NotImplementedException(),
            };
    }
}
