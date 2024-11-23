namespace Lab2.Domain.Models;

public record ValidatedOrderLine(Code Code,OrderId? OrderId, ProductId? ProductId, Quantity? Quantity, Price? Price)
{
}