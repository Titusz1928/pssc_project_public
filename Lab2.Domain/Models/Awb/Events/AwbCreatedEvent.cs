using System;
using System.Collections.Generic;

namespace Lab2.Domain.Models
{

    public static class AwbCreatedEvent
    {
        public interface IAwbCreatedEvent
        {
            // Common properties or methods can go here if needed for all delivery events
        }

        public record DeliverySucceededEvent : IAwbCreatedEvent
        {
            public OrderId OrderId { get; }
            public string DeliveryAddress { get; }
            public DateTime DeliveryDate { get; }

            internal DeliverySucceededEvent(OrderId oid, string deliveryAddress, DateTime deliveryDate)
            {
                OrderId = oid;
                DeliveryAddress = deliveryAddress;
                DeliveryDate = deliveryDate;
            }
        }

        public record DeliveryFailedEvent : IAwbCreatedEvent
        {
            public IEnumerable<string> Reasons { get; }

            internal DeliveryFailedEvent(string reason)
            {
                Reasons = new[] { reason };
            }

            internal DeliveryFailedEvent(IEnumerable<string> reasons)
            {
                Reasons = reasons;
            }
        }

        public static IAwbCreatedEvent ToEvent(this Awb.IAwb awb) =>
            awb switch
            {
                Awb.UnvalidatedAwb _ => new DeliveryFailedEvent("unexpected unvalidated state"),
                Awb.ValidatedAwb _ => new DeliveryFailedEvent("unexpected validated state"),
                Awb.InvalidAwb invalidAwb => new DeliveryFailedEvent(invalidAwb.Reasons),
                Awb.FinalizedAwb finalizedAwb => new DeliverySucceededEvent(finalizedAwb.AwbOrderInfo.OrderHeader.OrderId,
                    finalizedAwb.AwbOrderInfo.OrderHeader.Address, finalizedAwb.AwbOrderInfo.OrderDate),
                _ => throw new NotImplementedException(),
            };
    }
}