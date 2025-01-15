using Lab2.Data.Repositories;
using Lab2.Domain.Models;
using Lab2.Domain.Operations;
using Lab2.Domain.Repositories;
using Microsoft.Extensions.Logging;
using static Lab2.Domain.Models.Awb;

namespace Lab2.Domain.Workflows
{

    public class DeliveryWorkflow
    {
        private readonly IAwbRepository awbRepository;
        private readonly IUsersRepository usersRepository;
        private readonly ILogger<DeliveryWorkflow> logger;

        public DeliveryWorkflow(IAwbRepository awbRepository,IUsersRepository usersRepository, ILogger<DeliveryWorkflow> logger)
        {
            this.awbRepository = awbRepository;
            this.usersRepository = usersRepository;
            this.logger = logger;
        }

        public async Task<AwbCreatedEvent.IAwbCreatedEvent> ExecuteAsync(CreateAwbCommand command)
        {
            try
            {
                
                // Business logic execution
                Awb.IAwb awb = ExecuteBusinessLogic(command);

                if (awb is Awb.FinalizedAwb finalizedAwb)
                {
                    Console.WriteLine("Saving awb...");

                    // Save order header
                    await awbRepository.AddAwb(finalizedAwb);
                }

                return AwbCreatedEvent.ToEvent(awb);  // Ensure this returns the correct IAwbCreatedEvent
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while creating the order");
                return new AwbCreatedEvent.DeliveryFailedEvent("Unexpected error");  // Adjust the error event type
            }
        }

        private IAwb ExecuteBusinessLogic(CreateAwbCommand command)
        {
            // Use the repository to check if the email and phone number exist
            Func<string, Task<bool>> checkIfEmailExists = async email =>
            {
                return await usersRepository.EmailExistsAsync(email);
            };

            Func<string, Task<bool>> checkIfPhoneNrExists = async phoneNr =>
            {
                return await usersRepository.PhoneNrExistsAsync(phoneNr);
            };

            // Create the ValidateAwbOperation with the above functions
            IAwb awb = new ValidateAwbOperation(checkIfEmailExists, checkIfPhoneNrExists).Transform(command.UnvalidatedAwb);

            if (awb is Awb.ValidatedAwb validatedAwb)
            {
                awb = new FinalizeAwbOperation().Transform(awb);
            }

            return awb;
        }
        
    }
}