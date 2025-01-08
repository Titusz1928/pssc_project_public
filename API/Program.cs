using Lab2.API.Clients;
using Lab2.Data;
using Lab2.Data.Repositories;
using Lab2.Domain.Repositories;
using Lab2.Domain.Workflows;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;


namespace Lab2.API
{
    public class Program
    {
        public static void Main(string[] args)
        {


            WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            
            builder.Services.AddDbContext<OrderLineContext>(options =>
                options.UseMySql(
                    builder.Configuration.GetConnectionString("DefaultConnection"),
                    new MySqlServerVersion(new Version(8, 0, 2)) // Specify the MySQL version here
                )
            );
            
            builder.Services.AddTransient<IOrderLineRepository, OrderLineRepository>();
            builder.Services.AddTransient<IProductsRepository, ProductsRepository>();
            builder.Services.AddTransient<IOrderHeaderRepository, OrderHeaderRepository>();
            builder.Services.AddTransient<CreateOrderWorkflow>();
            builder.Services.AddTransient<CalculateOrderWorkflow>();
            builder.Services.AddTransient<PayOrderWorkflow>();
            builder.Services.AddTransient<PlaceOrderWorkflow>();

            builder.Services.AddHttpClient();
            
            builder.Services.AddControllers();
            
            
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Example.Api", Version = "v1" });
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();
            
            app.Run();
            

        }
        
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHttpClient<MyApiClient>(client =>
            {
                client.BaseAddress = new Uri("https://api.example.com/");
                client.DefaultRequestHeaders.Add("Accept", "application/json");
            });
        }

    }
}