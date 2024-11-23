namespace Lab2.Domain.Models;

public record UnvalidatedOrderLine(string Code,string? OrderId, string? ProductId, string? Quantity, string? Price)
{
}