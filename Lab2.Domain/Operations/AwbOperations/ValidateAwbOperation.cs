using System.Text.RegularExpressions;
using Lab2.Domain.Models;
using Lab2.Domain.Models.AwbContactInfo;
using System.Threading.Tasks;

namespace Lab2.Domain.Operations
{
    internal sealed class ValidateAwbOperation : AwbOperation
    {
        // Validate AwbContactInfo operation
        private readonly Func<string, Task<bool>> checkIfEmailExists;
        private readonly Func<string, Task<bool>> checkIfPhoneNrExists;

        internal ValidateAwbOperation(Func<string, Task<bool>> checkIfEmailExists, Func<string, Task<bool>> checkIfPhoneNrExists)
        {
            this.checkIfEmailExists = checkIfEmailExists;
            this.checkIfPhoneNrExists = checkIfPhoneNrExists;
        }
        
        protected override Awb.IAwb OnUnvalidated(Awb.UnvalidatedAwb unvalidatedAwb)
        {
            // Validate email and phone number synchronously
            var (validatedAwbContactInfo, validationErrors) = ValidateAwbContactInfo(unvalidatedAwb.UnvalidatedAwbContactInfo).Result;

            if (validationErrors.Any())
            {
                // Return an invalid contact info if there are errors
                return new Awb.InvalidAwb(
                    unvalidatedAwb.UnvalidatedAwbContactInfo, 
                    validationErrors,
                    unvalidatedAwb.AwbOrderInfo 
                );
            }
            else
            {
                // Return the validated contact info
                return new Awb.ValidatedAwb(
                    validatedAwbContactInfo,
                    unvalidatedAwb.AwbOrderInfo 
                );
            }
        }

        private async Task<(ValidatedAwbContactInfo?, IEnumerable<string>)> ValidateAwbContactInfo(UnvalidatedAwbContactInfo unvalidatedAwbContactInfo)
        {
            List<string> validationErrors = new();

            Email? email = ValidateAndParseEmail(unvalidatedAwbContactInfo.Email, validationErrors);
            PhoneNr? phoneNr = ValidateAndParsePhoneNr(unvalidatedAwbContactInfo.PhoneNr, validationErrors);

            // If there are validation errors, return them
            if (validationErrors.Any())
            {
                return (null, validationErrors);
            }

            // If no errors, return the valid AwbContactInfo
            var validatedAwbContactInfo = new ValidatedAwbContactInfo(email!, phoneNr!);
            return (validatedAwbContactInfo, validationErrors);
        }

        private Email? ValidateAndParseEmail(string email, List<string> validationErrors)
        {
            Email? parsedEmail;
            if (!Email.TryParse(email, out parsedEmail))
            {
                validationErrors.Add($"Invalid email: {email}");
            }
            else if (!checkIfEmailExists(email).Result) // Ensure to await the async check synchronously
            {
                validationErrors.Add($"Email doesn't exist: {email}");
            }

            return parsedEmail;
        }

        private PhoneNr? ValidateAndParsePhoneNr(string phoneNr, List<string> validationErrors)
        {
            PhoneNr? parsedPhoneNr;
            if (!PhoneNr.TryParse(phoneNr, out parsedPhoneNr))
            {
                validationErrors.Add($"Invalid phone number: {phoneNr}");
            }
            else if (!checkIfPhoneNrExists(phoneNr).Result) // Ensure to await the async check synchronously
            {
                validationErrors.Add($"Phone number doesn't exist: {phoneNr}");
            }

            return parsedPhoneNr;
        }
    }
}
