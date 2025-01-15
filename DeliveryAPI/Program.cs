using Lab2.Data;
using Lab2.Data.Repositories;
using Lab2.Domain.Repositories;
using Lab2.Domain.Workflows;
using Lab2.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

namespace Lab2.DeliveryAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            // Register DbContext (you'll need to provide a connection string here)
            builder.Services.AddDbContext<OrderLineContext>(options =>
                options.UseMySql(builder.Configuration.GetConnectionString("DefaultConnection"), 
                    ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("DefaultConnection"))));

            // Register repositories and workflows
            builder.Services.AddScoped<IAwbRepository, AwbRepository>(); 
            builder.Services.AddScoped<IUsersRepository, UsersRepository>();
            builder.Services.AddScoped<DeliveryWorkflow>();

            // Register other services
            builder.Services.AddControllers();

            // Swagger/OpenAPI configuration
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Delivery.Api", Version = "v1" });
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
    }
}