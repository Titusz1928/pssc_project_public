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
    
    public DbSet<OrderHeaderDto> OrderHeaders { get; set; }
    
    public DbSet<AwbDto> Awbs { get; set; }
    
    public DbSet<UserDto> Users { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Configure ProductDto
        modelBuilder
            .Entity<ProductDto>()
            .ToTable("Product")
            .HasKey(s => s.ProductId);
        
        // Configure ProductDto
        modelBuilder
            .Entity<OrderHeaderDto>()
            .ToTable("OrderHeader")
            .HasKey(s => s.OrderId);

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
                    .Property(g => g.Total)
                    .HasColumnName("Total");

                entityBuilder
                    .ToTable("OrderLine")
                    .HasKey(s => s.OrderLineId);
            });
        
        
        // Configure AwbDto (your Awb table)
        modelBuilder
            .Entity<AwbDto>()
            .ToTable("Awb")  // Set the table name
            .HasKey(s => s.OrderId);  // Assuming OrderId is the primary key

        // Configure AwbDto properties (adjust if needed)
        modelBuilder
            .Entity<AwbDto>()
            .Property(a => a.OrderId)
            .HasColumnType("int");

        modelBuilder
            .Entity<AwbDto>()
            .Property(a => a.Address)
            .HasMaxLength(255)
            .IsRequired();

        modelBuilder
            .Entity<AwbDto>()
            .Property(a => a.Name)
            .HasMaxLength(255)
            .IsRequired();

        modelBuilder
            .Entity<AwbDto>()
            .Property(a => a.Date)
            .IsRequired();

        modelBuilder
            .Entity<AwbDto>()
            .Property(a => a.Price)
            .IsRequired();

        modelBuilder
            .Entity<AwbDto>()
            .Property(a => a.Email)
            .HasMaxLength(255)
            .IsRequired();

        modelBuilder
            .Entity<AwbDto>()
            .Property(a => a.PhoneNr)
            .HasMaxLength(50)
            .IsRequired();
        
        // Configure UserDto (your Users table)
        modelBuilder
            .Entity<UserDto>()
            .ToTable("Users")  // Set the table name to Users
            .HasKey(u => u.email);  // Assuming email is the primary key

        modelBuilder
            .Entity<UserDto>()
            .Property(u => u.email)
            .HasMaxLength(255)
            .IsRequired();

        modelBuilder
            .Entity<UserDto>()
            .Property(u => u.phonenr)
            .HasMaxLength(50)
            .IsRequired();
    }
}
