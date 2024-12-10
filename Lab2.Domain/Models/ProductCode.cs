using Lab2.Domain.Exceptions;

namespace Lab2.Domain.Models
{
    public record ProductCode
    {
        public string Value { get; }

        public ProductCode(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new InvalidProductCodeException("Product code cannot be empty.");
            }

            Value = value;
        }
    }
}