using System;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using Lab2.Domain.Models;
using Lab2.Domain.Models.AwbContactInfo;
using static Lab2.Domain.Models.Order;

namespace Lab2.Domain.Operations
{
    internal sealed class FinalizeAwbOperation : AwbOperation
    {
        // Simulates sending an email
        private bool SendEmail(string email, out string reason)
        {
            // Fail if the email starts with 'f'
            if (email.StartsWith("f", StringComparison.OrdinalIgnoreCase))
            {
                reason = $"Email {email} starts with 'f'.";
                return false;
            }

            // Simulate the email sending process
            Console.WriteLine($"Simulating email sent to: {email}");
            reason = string.Empty;
            return !string.IsNullOrEmpty(email) && email.Contains('@');
        }

        // Simulates sending an SMS
        private bool SendSms(string phoneNr, out string reason)
        {
            // Fail if the second character in the phone number is '1'
            if (phoneNr.Length > 1 && phoneNr[1] == '1')
            {
                reason = $"Phone number {phoneNr} has '1' as the second character.";
                return false;
            }

            // Simulate the SMS sending process
            Console.WriteLine($"Simulating SMS sent to: {phoneNr}");
            reason = string.Empty;
            return !string.IsNullOrEmpty(phoneNr) && phoneNr.Length >= 10;
        }

        protected override Awb.IAwb OnValidated(Awb.ValidatedAwb validatedAwb)
        {
            var reasons = new List<string>();

            // Simulate sending email and SMS before finalizing the Awb
            var emailSent = SendEmail(validatedAwb.ValidatedAwbContactInfo.Email, out string emailReason);
            if (!emailSent)
            {
                reasons.Add(emailReason);
            }

            var smsSent = SendSms(validatedAwb.ValidatedAwbContactInfo.PhoneNr, out string smsReason);
            if (!smsSent)
            {
                reasons.Add(smsReason);
            }

            if (reasons.Any())
            {
                // Return an InvalidAwb if email or SMS fails with reasons
                Console.WriteLine("Failed to send email or SMS. Returning InvalidAwb.");
                UnvalidatedAwbContactInfo unvalidatedAwbContactInfo = new(validatedAwb.ValidatedAwbContactInfo.Email,validatedAwb.ValidatedAwbContactInfo.PhoneNr);
                return new Awb.InvalidAwb(unvalidatedAwbContactInfo, reasons, validatedAwb.AwbOrderInfo);
            }
            
            
            // Proceed to finalize the Awb if both email and SMS are successful
            FinalizedAwbContactInfo finalizedAwbContactInfo = new(validatedAwb.ValidatedAwbContactInfo.Email, validatedAwb.ValidatedAwbContactInfo.PhoneNr);
            
            Awb.FinalizedAwb finalizedAwb = new(finalizedAwbContactInfo, validatedAwb.AwbOrderInfo);
            return finalizedAwb; 
        }
    }
}
