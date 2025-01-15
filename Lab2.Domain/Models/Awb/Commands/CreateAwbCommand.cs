using Lab2.Domain.Models;
using Lab2.Domain.Repositories;
using Microsoft.Extensions.Logging;
using static Lab2.Domain.Models.Order;

namespace Lab2.Domain.Models
{

    public record CreateAwbCommand
    {
        public CreateAwbCommand(Awb.UnvalidatedAwb unvalidatedAwb)
        {
            UnvalidatedAwb = unvalidatedAwb;
        }

        public Awb.UnvalidatedAwb UnvalidatedAwb { get; }
    }
}