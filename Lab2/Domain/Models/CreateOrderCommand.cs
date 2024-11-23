using Lab2.Domain.Models;
using Lab2.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace Lab2.Domain.Models
{

    public record CreateOrderCommand
    {
        public CreateOrderCommand(IReadOnlyCollection<UnvalidatedOrderLine> inputOrderLines)
        {
            InputOrderLines = inputOrderLines;
        }

        public IReadOnlyCollection<UnvalidatedOrderLine> InputOrderLines { get; }
    }
}