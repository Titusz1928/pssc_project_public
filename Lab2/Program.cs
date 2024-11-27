using Lab2.Domain.Models;
using Lab2.Data.Models;
using Lab2.Data.Repositories;
using Lab2.Data;
using Lab2.Domain.Workflows;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using static Lab2.Domain.Models.OrderCreatedEvent;

namespace Lab2
{
    internal class Program
    {
        
        private static readonly string ConnectionString = "Server=localhost;Database=pssc3;Uid=root;Pwd=iNdigo24H+9jq!Zy3;";


        private static async Task Main(string[] args)
        {
            using ILoggerFactory loggerFactory = ConfigureLoggerFactory();
            ILogger<CreateOrderWorkflow> logger = loggerFactory.CreateLogger<CreateOrderWorkflow>();
            
            DbContextOptionsBuilder<OrderLineContext> dbContextBuilder = new DbContextOptionsBuilder<OrderLineContext>()
                .UseMySql(ConnectionString, ServerVersion.AutoDetect(ConnectionString)) // Using MySQL instead of SQL Server
                .UseLoggerFactory(loggerFactory);
            
            OrderLineContext orderLineContext = new OrderLineContext(dbContextBuilder.Options);
            
            ProductsRepository productsRepository = new(orderLineContext);
            OrderLineRepository orderLineRepository = new(orderLineContext);
            OrderHeaderRepository orderHeaderRepository = new(orderLineContext);
            
            //get user input
            UnvalidatedOrderLine[] listoforders = ReadListOfOrders().ToArray();
            OrderHeader header = ReadOrderHeader();
            
            //execute domain workflow
            CreateOrderCommand command = new(listoforders, header);
            CreateOrderWorkflow workflow = new(productsRepository, orderLineRepository,orderHeaderRepository, logger);
            IOrderCreatedEvent result = await workflow.ExecuteAsync(command);

            string consoleMessage = result switch
            {
                OrderCreationSucceededEvent @event => @event.Csv,
                OrderCreationFailedEvent @event => $"Order failed: \r\n{string.Join("\r\n", @event.Reasons)}",
                _ => throw new NotImplementedException()
            };
            
            Console.WriteLine();
            Console.WriteLine("============================");
            Console.WriteLine("Your order:");
            Console.WriteLine("============================");

            Console.WriteLine(consoleMessage);
        }
        
        private static ILoggerFactory ConfigureLoggerFactory()
        {
            return LoggerFactory.Create(builder =>
                builder.AddSimpleConsole(options =>
                    {
                        options.IncludeScopes = true;
                        options.SingleLine = true;
                        options.TimestampFormat = "hh:mm:ss ";
                    })
                    .AddProvider(new Microsoft.Extensions.Logging.Debug.DebugLoggerProvider()));
        }
        
        private static readonly Dictionary<int, float> ProductPrices = new()
        {
            { 1, 10.5f }, // ProductId 1 -> Price 10.5
            { 2, 20.0f }, // ProductId 2 -> Price 20.0
            // Add more product-price mappings as needed
        };
        
        private static OrderHeader ReadOrderHeader()
        {
            string name = ReadValue("Customer Name: ") ?? string.Empty;
            string city = ReadValue("Customer City: ") ?? string.Empty;
            return new OrderHeader(name, city,0);
        }
        
        private static List<UnvalidatedOrderLine> ReadListOfOrders()
        {
            List<UnvalidatedOrderLine> listOfOrders = new();
            do
            {
                // Read ProductId
                string? productIdInput = ReadValue("Product Id: ");
                if (string.IsNullOrEmpty(productIdInput))
                {
                    break;
                }

                if (!int.TryParse(productIdInput, out int productId) || !ProductPrices.ContainsKey(productId))
                {
                    Console.WriteLine("Invalid Product Id or Product not found.");
                    continue;
                }
                

                // Read Quantity
                string? quantityInput = ReadValue("Quantity: ");
                if (string.IsNullOrEmpty(quantityInput) || !int.TryParse(quantityInput, out int quantity))
                {
                    Console.WriteLine("Invalid Quantity.");
                    continue;
                }

                // Assign price based on ProductId
                float price = ProductPrices[productId];

                // Add the order line to the list
                listOfOrders.Add(new("0",productIdInput, quantity.ToString(), price.ToString()));
            } while (true);
            return listOfOrders;
        }
        
        private static string? ReadValue(string prompt)
        {
            Console.Write(prompt);
            return Console.ReadLine();
        }

    }
}
