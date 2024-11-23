namespace Lab2.Domain.Models;

public record CalculatedOrderLine(Code Code,OrderId? OrderId, ProductId? ProductId, Quantity? Quantity, Price? Price, Total? Total)
{
    public override string ToString()
    {
        return $"Code: {Code}, OrderId: {OrderId}, ProductId: {ProductId}, Quantity: {Quantity}, Price: {Price}, Total: {Total}";
    }
}