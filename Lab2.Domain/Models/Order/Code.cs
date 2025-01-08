using Lab2.Domain.Exceptions;

namespace Lab2.Domain.Models;
using System.Text.RegularExpressions;

public class Code
{
    private static readonly Regex ValidPattern = new("^PR[0-9]{3}$");
    public string Value { get; }

    internal Code(string value)
    {
        if (IsValid(value))
        {
            Value = value;
        }
        else
        {
            throw new InvalidProductCodeException("");
        }
    }
    
    private static bool IsValid(string value) => ValidPattern.IsMatch(value);
    
    public override string ToString()
    {
        return Value;
    }

    public static bool TryParse(string stringValue, out Code? code)
    {
        bool isValid = false;
        code = null;

        if (IsValid(stringValue))
        {
            isValid = true;
            code = new(stringValue);
        }
        return isValid;
    }
    
    public override bool Equals(object obj)
    {
        if (obj is Code otherCode)
        {
            return this.Value.Equals(otherCode.Value, StringComparison.OrdinalIgnoreCase);
        }
        return false;
    }

    public override int GetHashCode()
    {
        return Value?.GetHashCode(StringComparison.OrdinalIgnoreCase) ?? 0;
    }
}