using Lab2.Data.Models;
using Lab2.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Lab2.Data;

public class OrderLineContext : DbContext
{
    public OrderLineContext(DbContextOptions<OrderLineContext> options)
        : base(options) // Pass the options to the base DbContext
    {
    }

    public DbSet<OrderLineDto> Orders { get; set; }
    public DbSet<ProductDto> Products { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Configure ProductDto
        modelBuilder
            .Entity<ProductDto>()
            .ToTable("Product")
            .HasKey(s => s.ProductId);

        // Configure OrderLineDto
        modelBuilder
            .Entity<OrderLineDto>(entityBuilder =>
            {
                entityBuilder
                    .Property(g => g.OrderId)
                    .HasColumnType("int");

                entityBuilder
                    .Property(g => g.Quantity)
                    .HasColumnType("int");

                entityBuilder
                    .Property(g => g.Price)
                    .HasColumnName("Price");

                entityBuilder
                    .Property(g => g.Total)
                    .HasColumnName("Total");

                entityBuilder
                    .ToTable("OrderLine")
                    .HasKey(s => s.OrderLineId);
            });
    }
}
