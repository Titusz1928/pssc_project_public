namespace Lab2.Domain.Models
{
    public static class Order
    {
        public interface IOrder
        {
        }

        public record UnvalidatedOrder : IOrder
        {
            public UnvalidatedOrder(IReadOnlyCollection<UnvalidatedOrderLine> orderList, OrderHeader? header = null)
            {
                OrderList = orderList;
                Header = header;
            }

            public IReadOnlyCollection<UnvalidatedOrderLine> OrderList { get; }
            public OrderHeader? Header { get; }
        }

        public record InvalidOrder : IOrder
        {
            internal InvalidOrder(IReadOnlyCollection<UnvalidatedOrderLine> orderList, IEnumerable<string> reasons, OrderHeader? header = null)
            {
                OrderList = orderList;
                Reasons = reasons;
                Header = header;
            }

            public IReadOnlyCollection<UnvalidatedOrderLine> OrderList { get; }
            public IEnumerable<string> Reasons { get; }
            public OrderHeader? Header { get; }
        }

        public record ValidatedOrder : IOrder
        {
            internal ValidatedOrder(IReadOnlyCollection<ValidatedOrderLine> orderList, OrderHeader header)
            {
                OrderList = orderList;
                Header = header;
            }

            public IReadOnlyCollection<ValidatedOrderLine> OrderList { get; }
            public OrderHeader Header { get; }
        }

        public record CalculatedOrder : IOrder
        {
            internal CalculatedOrder(IReadOnlyCollection<CalculatedOrderLine> orderList, OrderHeader header)
            {
                OrderList = orderList;
                Header = header;
            }

            public IReadOnlyCollection<CalculatedOrderLine> OrderList { get; }
            public OrderHeader Header { get; }
            
            public override string ToString()
            {
                // Join all order lines as a string and include the header
                var orderLines = string.Join(", ", OrderList.Select(line => line.ToString()));
                return $"Header: {Header}, OrderList: [{orderLines}]";
            }
        }

        public record PayedOrder : IOrder
        {
            internal PayedOrder(IReadOnlyCollection<CalculatedOrderLine> orderList, OrderHeader header, string csv, DateTime date)
            {
                OrderList = orderList;
                Header = header;
                CreatedDate = date;
                Csv = csv;
            }

            public IReadOnlyCollection<CalculatedOrderLine> OrderList { get; }
            public OrderHeader Header { get; }
            public DateTime CreatedDate { get; }
            public string Csv { get; }
            
            public override string ToString()
            {
                // Custom ToString implementation for debugging
                var orderListString = string.Join(", ", OrderList.Select(line => $"ProductId: {line.ProductId}, Quantity: {line.Quantity}, Total: {line.Total}"));
                return $"PayedOrder {{ Header: {Header}, OrderList: [{orderListString}], CreatedDate: {CreatedDate}, Csv: {Csv} }}";
            }
        }
    }
}
