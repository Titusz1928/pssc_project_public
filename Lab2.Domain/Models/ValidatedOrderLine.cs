namespace Lab2.Domain.Models;

public record ValidatedOrderLine(OrderId? OrderId, ProductId? ProductId, Quantity? Quantity, Price? Price)
{
}