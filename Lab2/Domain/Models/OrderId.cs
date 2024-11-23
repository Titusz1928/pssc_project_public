using Lab2.Domain.Exceptions;

namespace Lab2.Domain.Models;

public record OrderId
{
    public int Value { get; }

    private OrderId()
    {
        Value = 0;
    }

    public OrderId(int value)
    {
        if (IsValid(value))
        {
            Value = value;
        }
        else
        {
            throw new InvalidOrderIdException($"{value} is an invalid OrderId value.");
        }
    }

    public override string ToString() => Value.ToString();

    public static bool TryParse(string? orderIdString, out OrderId? orderId)
    {
        bool isValid = false;
        orderId = null;

        if (int.TryParse(orderIdString, out int numericOrderId))
        {
            if (IsValid(numericOrderId))
            {
                isValid = true;
                orderId = new OrderId(numericOrderId);
            }
        }

        return isValid;
    }

    private static bool IsValid(int numericOrderId) => numericOrderId > 0;
}