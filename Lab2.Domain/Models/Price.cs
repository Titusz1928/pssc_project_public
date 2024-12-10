using Lab2.Domain.Exceptions;

namespace Lab2.Domain.Models;

public record Price
{

    public float Value { get; }

    private Price()
    {
        Value = 0;
    }

    public Price(float value)
    {
        if (IsValid(value))
        {
            Value = value;
        }
        else
        {
            throw new InvalidPriceException($"{value:0.##} is an invalid price value.");
        }
    }

    public static Price operator +(Price a, Price b) => new((a.Value + b.Value) / 2f);


    public Price Round()
    {
        float roundedValue = (float)Math.Round(Value, MidpointRounding.AwayFromZero);
        return new Price(roundedValue);
    }

    public override string ToString() => $"{Value:0.##}";

    public static bool TryParsePrice(string? priceString, out Price? price)
    {
        bool isValid = false;
        price = null;
        if (float.TryParse(priceString, out float numericPrice))
        {
            if (IsValid(numericPrice))
            {
                isValid = true;
                price = new(numericPrice);
            }
        }

        return isValid;
    }

    private static bool IsValid(float numericPrice) => numericPrice > 0 && numericPrice <= 100000;
}