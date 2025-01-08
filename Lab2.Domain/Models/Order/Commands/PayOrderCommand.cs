namespace Lab2.Domain.Models
{

    public class PayOrderCommand
    {
        public PayOrderCommand(IReadOnlyCollection<CalculatedOrderLine> inputOrderLines,  OrderHeader header)
        {
            InputOrderLines = inputOrderLines;
            Header = header;
        }

        public IReadOnlyCollection<CalculatedOrderLine> InputOrderLines { get; }
        public OrderHeader Header { get; }
    }
}