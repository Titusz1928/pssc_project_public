using Lab2.Domain.Exceptions;
using System.Text.RegularExpressions;

namespace Lab2.Domain.Models
{
    public record PhoneNr
    {
        public string Value { get; }

        private PhoneNr()
        {
            Value = string.Empty;
        }

        public PhoneNr(string value)
        {
            if (IsValid(value))
            {
                Value = value;
            }
            else
            {
                throw new InvalidPhoneNrException($"{value} is an invalid phone number.");
            }
        }

        public override string ToString() => Value;

        public static bool TryParse(string? phoneNrString, out PhoneNr? phoneNr)
        {
            phoneNr = null;
            if (!string.IsNullOrWhiteSpace(phoneNrString) && IsValid(phoneNrString))
            {
                phoneNr = new PhoneNr(phoneNrString);
                return true;
            }
            return false;
        }

        private static bool IsValid(string phoneNr)
        {
            // Basic regex for phone number validation
            var phoneNrRegex = new Regex(@"^\+?[1-9]\d{1,14}$");
            return phoneNrRegex.IsMatch(phoneNr);
        }

        // Implicit conversion from string to PhoneNr
        public static implicit operator PhoneNr(string value)
        {
            return new PhoneNr(value);
        }

        // Implicit conversion from PhoneNr to string
        public static implicit operator string(PhoneNr phoneNr)
        {
            return phoneNr.Value;
        }
    }
}