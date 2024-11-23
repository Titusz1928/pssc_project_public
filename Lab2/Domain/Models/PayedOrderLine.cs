namespace Lab2.Domain.Models;

public record PayedOrderLine(Code Code,OrderId OrderId, ProductId ProductId, Quantity Quantity, Price Price, Total Total)
{
    public int OrderLineId { get; set; }
    public bool IsUpdated { get; set; }
}