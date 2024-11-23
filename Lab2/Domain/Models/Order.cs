namespace Lab2.Domain.Models
{


    public static class Order
    {
        public interface IOrder
        {
        }

        public record UnvalidatedOrder : IOrder
        {
            public UnvalidatedOrder(IReadOnlyCollection<UnvalidatedOrderLine> orderList)
            {
                OrderList = orderList;
            }

            public IReadOnlyCollection<UnvalidatedOrderLine> OrderList { get; }
        }

        public record InvalidOrder : IOrder
        {
            internal InvalidOrder(IReadOnlyCollection<UnvalidatedOrderLine> orderList, IEnumerable<string> reasons)
            {
                OrderList = orderList;
                Reasons = reasons;
            }

            public IReadOnlyCollection<UnvalidatedOrderLine> OrderList { get; }
            public IEnumerable<string> Reasons { get; }
        }

        public record ValidatedOrder : IOrder
        {
            internal ValidatedOrder(IReadOnlyCollection<ValidatedOrderLine> orderList)
            {
                OrderList = orderList;
            }

            public IReadOnlyCollection<ValidatedOrderLine> OrderList { get; }
        }

        public record CalculatedOrder : IOrder
        {
            internal CalculatedOrder(IReadOnlyCollection<CalculatedOrderLine> orderList)
            {
                OrderList = orderList;
            }

            public IReadOnlyCollection<CalculatedOrderLine> OrderList { get; }
        }


        public record PayedOrder : IOrder
        {
            internal PayedOrder(IReadOnlyCollection<CalculatedOrderLine> orderList, string csv, DateTime date)
            {
                OrderList = orderList;
                CreatedDate = date;
                Csv = csv;
            }

            public IReadOnlyCollection<CalculatedOrderLine> OrderList { get; }
            public DateTime CreatedDate { get; }
            public string Csv { get; }
        }
    }
}