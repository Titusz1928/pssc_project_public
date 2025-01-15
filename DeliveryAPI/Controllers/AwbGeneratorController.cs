using Lab2.Domain.Models;
using Lab2.Domain.Workflows;
using Microsoft.AspNetCore.Mvc;

namespace Lab2.DeliveryAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AwbGeneratorController : ControllerBase
    {
        private readonly DeliveryWorkflow deliveryWorkflow;

        public AwbGeneratorController(DeliveryWorkflow deliveryWorkflow)
        {
            this.deliveryWorkflow = deliveryWorkflow;
        }

        /// <summary>
        /// Executes the AWB delivery workflow with the provided command.
        /// </summary>
        /// <param name="command">The CreateAwbCommand containing AWB details.</param>
        /// <returns>An action result indicating success or failure.</returns>
        [HttpPost("generate")]
        public async Task<IActionResult> GenerateAwb([FromBody] CreateAwbCommand command)
        {
            if (command == null || command.UnvalidatedAwb == null)
            {
                return BadRequest("Invalid AWB command or data.");
            }

            try
            {
                // Execute the workflow
                var result = await deliveryWorkflow.ExecuteAsync(command);

                // Return appropriate responses based on the result
                return result switch
                {
                    AwbCreatedEvent.DeliverySucceededEvent success =>
                        Ok(new { Message = "AWB created successfully."+success.OrderId+" "+success.DeliveryAddress+" "+success.DeliveryDate}),
                    AwbCreatedEvent.DeliveryFailedEvent failure =>
                        BadRequest(new { Message = failure.Reasons }),
                    _ => StatusCode(500, "Unexpected workflow result.")
                };
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred.", Error = ex.Message });
            }
        }
    }
}