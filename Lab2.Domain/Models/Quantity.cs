using Lab2.Domain.Exceptions;

namespace Lab2.Domain.Models
{
    public record Quantity
    {
        public int Value { get; }

        private Quantity()
        {
            Value = 0;
        }

        public Quantity(int value)
        {
            if (IsValid(value))
            {
                Value = value;
            }
            else
            {
                throw new InvalidQuantityException($"{value} is an invalid Quantity value.");
            }
        }

        public override string ToString() => Value.ToString();

        public static bool TryParse(string? quantityString, out Quantity? quantity)
        {
            bool isValid = false;
            quantity = null;

            if (int.TryParse(quantityString, out int numericQuantity))
            {
                if (IsValid(numericQuantity))
                {
                    isValid = true;
                    quantity = new Quantity(numericQuantity);
                }
            }

            return isValid;
        }

        private static bool IsValid(int numericQuantity) => numericQuantity >= 0;
    }
}