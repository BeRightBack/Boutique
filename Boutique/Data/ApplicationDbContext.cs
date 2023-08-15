using Boutique.Areas.Admin.Models.ToDo;
using Boutique.Entity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Boutique.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : base(options)
    { }

    public DbSet<Todo> ToDos { get; set; }
    public DbSet<ToDoCategory> ToDoCategories { get; set; }
    public DbSet<ToDoStatus> ToDoStatuses { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<ApplicationUser>(entity =>
        {
            entity.ToTable(name: "Users");
        });
        builder.Entity<IdentityRole>(entity =>
        {
            entity.ToTable(name: "Roles");
        });
        builder.Entity<IdentityUserRole<string>>(entity =>
        {
            entity.ToTable("UserRoles");
        });
        builder.Entity<IdentityUserClaim<string>>(entity =>
        {
            entity.ToTable("UserClaims");
        });
        builder.Entity<IdentityUserLogin<string>>(entity =>
        {
            entity.ToTable("UserLogins");
        });
        builder.Entity<IdentityRoleClaim<string>>(entity =>
        {
            entity.ToTable("RoleClaims");
        });
        builder.Entity<IdentityUserToken<string>>(entity =>
        {
            entity.ToTable("UserTokens");
        });

        builder.Entity<ToDoCategory>().HasData(
            new ToDoCategory { Id = "Identity", Name = "Identity" },
            new ToDoCategory { Id = "Store", Name = "Store" },
            new ToDoCategory { Id = "Localization", Name = "Localization" },
            new ToDoCategory { Id = "Layout", Name = "Layout" },
            new ToDoCategory { Id = "Admin", Name = "Administration" }
        );

        builder.Entity<ToDoStatus>().HasData(
            new ToDoStatus { Id = "open", Name = "Open" },
            new ToDoStatus { Id = "inprogress", Name = "In Progress" },
            new ToDoStatus { Id = "closed", Name = "Closed" }
        );

    }
}