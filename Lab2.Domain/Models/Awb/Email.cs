using Lab2.Domain.Exceptions;
using System.Text.RegularExpressions;

namespace Lab2.Domain.Models
{
    public record Email
    {
        public string Value { get; }

        private Email()
        {
            Value = string.Empty;
        }

        public Email(string value)
        {
            if (IsValid(value))
            {
                Value = value;
            }
            else
            {
                throw new InvalidEmailException($"{value} is an invalid email address.");
            }
        }

        public override string ToString() => Value;

        public static bool TryParse(string? emailString, out Email? email)
        {
            email = null;
            if (!string.IsNullOrWhiteSpace(emailString) && IsValid(emailString))
            {
                email = new Email(emailString);
                return true;
            }
            return false;
        }

        private static bool IsValid(string email)
        {
            // Simple regex for email validation
            var emailRegex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
            return emailRegex.IsMatch(email);
        }

        // Implicit conversion from string to Email
        public static implicit operator Email(string value)
        {
            return new Email(value);
        }

        // Implicit conversion from Email to string
        public static implicit operator string(Email email)
        {
            return email.Value;
        }
    }
}