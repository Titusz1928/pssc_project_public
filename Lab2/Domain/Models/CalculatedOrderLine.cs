namespace Lab2.Domain.Models;

public record CalculatedOrderLine(OrderId? OrderId, ProductId? ProductId, Quantity? Quantity, Total? Total)
{
}