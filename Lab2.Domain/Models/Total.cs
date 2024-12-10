using Lab2.Domain.Exceptions;

namespace Lab2.Domain.Models;

public class Total
{
    public float Value { get; }

    private Total()
    {
        Value = 0;
    }

    public Total(float value)
    {
        if (IsValid(value))
        {
            Value = value;
        }
        else
        {
            throw new InvalidTotalException($"{value:0.##} is an invalid price value.");
        }
    }

    public static Total operator +(Total a, Total b) => new((a.Value + b.Value) / 2f);


    public Total Round()
    {
        float roundedValue = (float)Math.Round(Value, MidpointRounding.AwayFromZero);
        return new Total(roundedValue);
    }

    public override string ToString() => $"{Value:0.##}";

    public static bool TryParseTotal(string? totalString, out Total? total)
    {
        bool isValid = false;
        total = null;
        if (float.TryParse(totalString, out float numericTotal))
        {
            if (IsValid(numericTotal))
            {
                isValid = true;
                total = new(numericTotal);
            }
        }

        return isValid;
    }

    private static bool IsValid(float numericTotal) => numericTotal > 0 && numericTotal <= 1000000;
}