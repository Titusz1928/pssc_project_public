using Lab2.Data.Models;
using Lab2.Domain.Models;
using Lab2.Domain.Repositories;
using System.Linq;

namespace Lab2.Data.Repositories
{
    public class AwbRepository : IAwbRepository
    {
        private readonly OrderLineContext dbContext;

        public AwbRepository(OrderLineContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task AddAwb(Awb.IAwb awb)
        {
            if (awb is Awb.FinalizedAwb finalizedAwb)
            {
                // Logging data before processing
                Console.WriteLine("Received finalized Awb object.");

                // Create the AwbDto object from the FinalizedAwb
                var awbDto = new AwbDto
                {
                    OrderId = finalizedAwb.AwbOrderInfo?.OrderHeader?.OrderId ?? throw new InvalidOperationException("OrderHeader is null."),
                    Address = finalizedAwb.AwbOrderInfo?.OrderHeader?.Address ?? string.Empty,
                    Name = finalizedAwb.AwbOrderInfo?.OrderHeader?.Name ?? string.Empty,
                    Date = finalizedAwb.AwbOrderInfo?.OrderDate ?? DateTime.MinValue,
                    Price = finalizedAwb.AwbOrderInfo?.OrderPrice?.Value ?? 0f,
                    Email = finalizedAwb.FinalizedAwbContactInfo?.Email ?? string.Empty,
                    PhoneNr = finalizedAwb.FinalizedAwbContactInfo?.PhoneNr ?? string.Empty
                };

                // Log the details of the AwbDto to ensure the values are being populated correctly
                Console.WriteLine($"Creating AwbDto: OrderId = {awbDto.OrderId}, Address = {awbDto.Address}, Name = {awbDto.Name}, " +
                                  $"Date = {awbDto.Date}, Price = {awbDto.Price}, Email = {awbDto.Email}, PhoneNr = {awbDto.PhoneNr}");

                // Add to the database
                await dbContext.Awbs.AddAsync(awbDto);
                
                // Log to confirm the object was added to the context
                Console.WriteLine("AwbDto added to DbContext.");
            }
            else
            {
                // Log if the awb is not of the expected type
                Console.WriteLine("Received an Awb that is not of type FinalizedAwb.");
            }

            // Attempt to save changes
            try
            {
                await dbContext.SaveChangesAsync();
                // Log after the data is saved to the database
                Console.WriteLine("Changes saved to the database.");
            }
            catch (Exception ex)
            {
                // Log any errors that occur while saving
                Console.WriteLine($"Error saving changes to the database: {ex.Message}");
            }
        }
        
        /*public async Task AddAwb(Awb.IAwb awb)
        {
            if (awb is Awb.ValidatedAwb validatedAwb)
            {
                // Logging data before processing
                Console.WriteLine("Received finalized Awb object.");

                // Create the AwbDto object from the FinalizedAwb
                var awbDto = new AwbDto
                {
                    OrderId = validatedAwb.AwbOrderInfo?.OrderHeader?.OrderId ?? throw new InvalidOperationException("OrderHeader is null."),
                    Address = validatedAwb.AwbOrderInfo?.OrderHeader?.Address ?? string.Empty,
                    Name = validatedAwb.AwbOrderInfo?.OrderHeader?.Name ?? string.Empty,
                    Date = validatedAwb.AwbOrderInfo?.OrderDate ?? DateTime.MinValue,
                    Price = validatedAwb.AwbOrderInfo?.OrderPrice?.Value ?? 0f,
                    Email = validatedAwb.ValidatedAwbContactInfo?.Email ?? string.Empty,
                    PhoneNr = validatedAwb.ValidatedAwbContactInfo?.PhoneNr ?? string.Empty
                };

                // Log the details of the AwbDto to ensure the values are being populated correctly
                Console.WriteLine($"Creating AwbDto: OrderId = {awbDto.OrderId}, Address = {awbDto.Address}, Name = {awbDto.Name}, " +
                                  $"Date = {awbDto.Date}, Price = {awbDto.Price}, Email = {awbDto.Email}, PhoneNr = {awbDto.PhoneNr}");

                // Add to the database
                await dbContext.Awbs.AddAsync(awbDto);
                
                // Log to confirm the object was added to the context
                Console.WriteLine("AwbDto added to DbContext.");
            }
            else
            {
                // Log if the awb is not of the expected type
                Console.WriteLine("Received an Awb that is not of type FinalizedAwb.");
            }

            // Attempt to save changes
            try
            {
                await dbContext.SaveChangesAsync();
                // Log after the data is saved to the database
                Console.WriteLine("Changes saved to the database.");
            }
            catch (Exception ex)
            {
                // Log any errors that occur while saving
                Console.WriteLine($"Error saving changes to the database: {ex.Message}");
            }
        }*/
    }
}
