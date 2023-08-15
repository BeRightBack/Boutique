using Boutique.Entity;
using Microsoft.EntityFrameworkCore;

namespace Boutique.Data;

public class LocalizationDbContext : DbContext
{
    public LocalizationDbContext(DbContextOptions<LocalizationDbContext> options)
    : base(options)
    {
    }

    public virtual DbSet<Language> Languages { get; set; }
    public virtual DbSet<StringResource> StringResources { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Language>(entity =>
        {
            entity.Property(e => e.Name).HasMaxLength(50);

            entity.Property(e => e.Name).HasMaxLength(50);
        });

        builder.Entity<StringResource>(entity =>
        {
            entity.Property(e => e.Name).HasMaxLength(50);

            entity.Property(e => e.Value).HasMaxLength(50);

            entity.HasOne(d => d.Language)
                .WithMany(p => p.StringResources)
                .HasForeignKey(d => d.LanguageId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}