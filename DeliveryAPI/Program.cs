using Lab2.Data;
using Lab2.Data.Repositories;
using Lab2.Domain.Repositories;
using Lab2.Domain.Workflows;
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
            
            
            
            
            
            builder.Services.AddControllers();
            
            
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
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