using Lab2.Domain.Models;
using Lab2.Domain.Repositories;
using Microsoft.Extensions.Logging;
using static Lab2.Domain.Models.Order;

namespace Lab2.Domain.Models
{

    public record PlaceOrderCommand
    {
        public PlaceOrderCommand(IReadOnlyCollection<UnvalidatedOrderLine> inputOrderLines,  OrderHeader header)
        {
            InputOrderLines = inputOrderLines;
            Header = header;
        }

        public IReadOnlyCollection<UnvalidatedOrderLine> InputOrderLines { get; }
        public OrderHeader Header { get; }
    }
}