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
        
        private static readonly string ConnectionString = "Server=localhost;Database=pssc;Uid=root;Pwd=iNdigo24H+9jq!Zy3;";


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
            
            //get user input
            UnvalidatedOrderLine[] listoforders = ReadListOfOrders().ToArray();
            
            //execute domain workflow
            CreateOrderCommand command = new(listoforders);
            CreateOrderWorkflow workflow = new(productsRepository, orderLineRepository, logger);
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
        
        private static List<UnvalidatedOrderLine> ReadListOfOrders()
        {
            List<UnvalidatedOrderLine> listOfGrades = [];
            do
            {
                string? code = ReadValue("Product Code: ");
                if (string.IsNullOrEmpty(code))
                {
                    break;
                }

                string? orderId = ReadValue("Order Id: ");
                if (string.IsNullOrEmpty(orderId))
                {
                    break;
                }

                string? productId = ReadValue("Product Id: ");
                if (string.IsNullOrEmpty(productId))
                {
                    break;
                }
                
                string? quantity = ReadValue("Quantity: ");
                if (string.IsNullOrEmpty(quantity))
                {
                    break;
                }
                
                string? price = ReadValue("Price: ");
                if (string.IsNullOrEmpty(quantity))
                {
                    break;
                }

                listOfGrades.Add(new(code, orderId, productId,quantity,price));
            } while (true);
            return listOfGrades;
        }
        
        private static string? ReadValue(string prompt)
        {
            Console.Write(prompt);
            return Console.ReadLine();
        }

    }
}
