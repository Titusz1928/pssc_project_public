using Lab2.Domain.Models;
using System.Collections.Generic;

using static Lab2.Domain.Models.Order;

namespace Lab2.Domain.Models
{
    public record CalculateOrderCommand
    {
        public CalculateOrderCommand(IReadOnlyCollection<UnvalidatedOrderLine> inputOrderLines,  OrderHeader header)
        {
            InputOrderLines = inputOrderLines;
            Header = header;
        }

        public IReadOnlyCollection<UnvalidatedOrderLine> InputOrderLines { get; }
        public OrderHeader Header { get; }
    }
}