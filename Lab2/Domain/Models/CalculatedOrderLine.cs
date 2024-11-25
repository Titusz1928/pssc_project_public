namespace Lab2.Domain.Models;

public record CalculatedOrderLine(OrderId? OrderId, ProductId? ProductId, Quantity? Quantity, Price? Price, Total? Total)
{
    public override string ToString()
    {
        return $"OrderId: {OrderId}, ProductId: {ProductId}, Quantity: {Quantity}, Price: {Price}, Total: {Total}";
    }
}