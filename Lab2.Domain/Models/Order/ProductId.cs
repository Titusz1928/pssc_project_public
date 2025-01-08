using Lab2.Domain.Exceptions;

namespace Lab2.Domain.Models;

public record ProductId
{
    public int Value { get; }

    private ProductId()
    {
        Value = 0;
    }

    public ProductId(int value)
    {
        if (IsValid(value))
        {
            Value = value;
        }
        else
        {
            throw new InvalidProductIdException($"{value} is an invalid ProductId value.");
        }
    }

    public override string ToString() => Value.ToString();

    public static bool TryParse(string? productIdString, out ProductId? productId)
    {
        bool isValid = false;
        productId = null;

        if (int.TryParse(productIdString, out int numericProductId))
        {
            if (IsValid(numericProductId))
            {
                isValid = true;
                productId = new ProductId(numericProductId);
            }
        }

        return isValid;
    }

    private static bool IsValid(int numericProductId) => numericProductId > 0;
}