using Boutique.Entity;
using Microsoft.EntityFrameworkCore;

namespace Boutique.Data;

public class CatalogDbContext : DbContext
{
    public CatalogDbContext(DbContextOptions<CatalogDbContext> options)
    : base(options)
    { }

    public DbSet<BillingAddress> BillingAddresses { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Image> Images { get; set; }
    public DbSet<Manufacturer> Manufacturers { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<ProductCategoryMapping> ProductCategoryMappings { get; set; }
    public DbSet<ProductImageMapping> ProductImageMappings { get; set; }
    public DbSet<ProductManufacturerMapping> ProductManufacturerMappings { get; set; }
    public DbSet<ProductSpecificationMapping> ProductSpecificationMappings { get; set; }
    public DbSet<Review> Reviews { get; set; }
    public DbSet<Specification> Specifications { get; set; }
    public DbSet<OrderCount> OrderCounts { get; set; }
    public DbSet<VisitorCount> VisitorCounts { get; set; }
    public DbSet<ContactUsMessage> ContactUsMessage { get; set; }
    public DbSet<Contact> Contact { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<BillingAddress>().ToTable("BillingAddress");
        modelBuilder.Entity<Category>().ToTable("Category");
        modelBuilder.Entity<Image>().ToTable("Image");
        modelBuilder.Entity<Manufacturer>().ToTable("Manufacturer");
        modelBuilder.Entity<Order>().ToTable("Order");
        modelBuilder.Entity<OrderItem>().ToTable("OrderItem");
        modelBuilder.Entity<Product>().ToTable("Product");
        modelBuilder.Entity<ProductCategoryMapping>().ToTable("ProductCategoryMapping");
        modelBuilder.Entity<ProductImageMapping>().ToTable("ProductImageMapping");
        modelBuilder.Entity<ProductManufacturerMapping>().ToTable("ProductManufacturerMapping");
        modelBuilder.Entity<ProductSpecificationMapping>().ToTable("ProductSpecificationMapping");
        modelBuilder.Entity<Review>().ToTable("Review");
        modelBuilder.Entity<Specification>().ToTable("Specification");

        modelBuilder.Entity<OrderCount>().ToTable("OrderCount");
        modelBuilder.Entity<VisitorCount>().ToTable("VisitorCount");

        modelBuilder.Entity<ContactUsMessage>().ToTable("ContactUsMessage");
    }


}