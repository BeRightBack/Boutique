using Boutique.Entity;
using Microsoft.EntityFrameworkCore;

namespace Boutique.Data;

public class DisplayHtmlDbContext : DbContext
{
    public DisplayHtmlDbContext(DbContextOptions<DisplayHtmlDbContext> options)
    : base(options)
    {
    }

    public DbSet<Content> Contents { get; set; }
    
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        
    }
}