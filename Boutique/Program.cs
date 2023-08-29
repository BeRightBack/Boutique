using System.Globalization;
using Serilog;
using Serilog.Events;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Identity;
using Boutique.Services;
using Boutique.Entity;
using Boutique.EFRepository;
using Boutique.Data;
using Boutique.Configuration;
using Boutique.Helpers;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.FileProviders;
using System.IO;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;
using Boutique.Areas.Editor.Data;
using Boutique.Areas.Editor.Services;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    .MinimumLevel.Override("System", LogEventLevel.Warning)
    .MinimumLevel.Override("Microsoft.AspNetCore.Authentication", LogEventLevel.Information)
    .Enrich.FromLogContext()
    .WriteTo.File(path: "logs/log-.txt",
        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}",
        rollingInterval: RollingInterval.Day,
        restrictedToMinimumLevel: LogEventLevel.Information
    ).CreateLogger();



var builder = WebApplication.CreateBuilder(args);

var identityString = builder.Configuration.GetConnectionString("IdentityConnection") ?? throw new InvalidOperationException("Connection string For Identity Connection not found.");
var layoutString = builder.Configuration.GetConnectionString("LayoutConnection") ?? throw new InvalidOperationException("Connection string For Layout Connection not found.");
var catalogString = builder.Configuration.GetConnectionString("CatalogConnection") ?? throw new InvalidOperationException("Connection string For Catalog Connection' not found.");
var localizationString = builder.Configuration.GetConnectionString("LocalizationConnection") ?? throw new InvalidOperationException("Connection string 'LocalizationConnection' not found.");

try
{
    Log.Information("Application is Starting");

    builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseSqlite(identityString));

    builder.Services.AddDbContext<DisplayHtmlDbContext>(options =>
        options.UseSqlite(layoutString));

    builder.Services.AddDbContext<CatalogDbContext>(options =>
        options.UseSqlite(catalogString));

    builder.Services.AddDbContext<LocalizationDbContext>(options =>
        options.UseSqlite(localizationString));

    builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
    {
        options.Password.RequireDigit = true;
        options.Password.RequiredLength = 8;
        options.Password.RequireNonAlphanumeric = true;
        options.Password.RequireUppercase = true;
        options.Password.RequireLowercase = true;
        options.Password.RequiredUniqueChars = 6;

        options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(30);
        options.Lockout.MaxFailedAccessAttempts = 10;
        options.Lockout.AllowedForNewUsers = true;

        options.User.RequireUniqueEmail = true;
        options.SignIn.RequireConfirmedAccount = true;
    })

        .AddEntityFrameworkStores<ApplicationDbContext>()
        .AddDefaultTokenProviders();

    builder.Services.ConfigureApplicationCookie(options =>
    {
        options.Cookie.HttpOnly = true;
        options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
        options.LoginPath = "/Account/Login"; // If the LoginPath is not set here, ASP.NET Core will default to /Account/Login
        options.LogoutPath = "/Account/Logout"; // If the LogoutPath is not set here, ASP.NET Core will default to /Account/Logout
        options.AccessDeniedPath = "/Account/AccessDenied"; // If the AccessDeniedPath is not set here, ASP.NET Core will default to /Account/AccessDenied
        options.SlidingExpiration = true;
    });

    builder.Services.AddDatabaseDeveloperPageExceptionFilter();

    builder.Services.AddScoped<IEmailSender, EmailSender>();
    builder.Services.AddScoped<ILocalizationService, LocalizationService>();
    builder.Services.AddScoped<ILanguageService, LanguageService>();
    builder.Services.AddHttpContextAccessor();

    builder.Services.AddAutoMapper(typeof(Program));
    builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
    builder.Services.AddScoped(typeof(IDisplayRepository<>), typeof(DisplayRepository<>));
    builder.Services.AddTransient<IBillingAddressService, BillingAddressService>();
    builder.Services.AddTransient<ICategoryService, CategoryService>();
    builder.Services.AddTransient<IImageManagerService, ImageManagerService>();
    builder.Services.AddTransient<IManufacturerService, ManufacturerService>();
    builder.Services.AddTransient<IOrderService, OrderService>();
    builder.Services.AddTransient<IProductService, ProductService>();
    builder.Services.AddTransient<IReviewService, ReviewService>();
    builder.Services.AddTransient<ISpecificationService, SpecificationService>();
    builder.Services.AddTransient<IDisplayService, DisplayService>();

    builder.Services.AddTransient<IOrderCountService, OrderCountService>();
    builder.Services.AddTransient<IVisitorCountService, VisitorCountService>();

    builder.Services.AddScoped<ViewHelper>();
    builder.Services.AddScoped<DataHelper>();

    builder.Services.AddSingleton<IFileProvider>(new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot")));

    builder.Services.AddTransient<SeedLanguage>();
    builder.Services.AddTransient<SeedData>();
    builder.Services.AddTransient<SeedProducts>();

    builder.Services.Configure<RequestLocalizationOptions>(options =>
        {
            var supportedCultures = new List<CultureInfo>
                {
                new CultureInfo("en"),
                new CultureInfo("fr"),
                new CultureInfo("es")
                };

            options.DefaultRequestCulture = new RequestCulture(culture: "fr", uiCulture: "fr");
            options.SupportedCultures = supportedCultures;
            options.SupportedUICultures = supportedCultures;

            options.RequestCultureProviders.Insert(0, new QueryStringRequestCultureProvider());
        });

    builder.Services.AddControllersWithViews()
    .AddViewLocalization()
        .AddDataAnnotationsLocalization();

    builder.Services.AddLocalization();

    builder.Services.AddDistributedMemoryCache();
    builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(60);
                options.Cookie.Name = "ShoppingCart";
            });



    var app = builder.Build();

    if (app.Environment.IsDevelopment())
    {
        //app.UseDeveloperExceptionPage();
    }
    else
    {
        //app.UseExceptionHandler("/Home/Error");
        //app.UseHsts();
    }

    app.UseDeveloperExceptionPage();
    app.UseHttpsRedirection();
    app.UseStaticFiles();


    var locOptions = ((IApplicationBuilder)app).ApplicationServices.GetRequiredService<IOptions<RequestLocalizationOptions>>();
    app.UseRequestLocalization(locOptions.Value);


    app.UseRouting();
    app.UseAuthentication();
    app.UseAuthorization();
    app.UseSession();

    // app.MapAreaControllerRoute(
    //     name: "Admin",
    //     areaName: "Admin",
    //     pattern: "Admin/{controller=Home}/{action=Index}/{id?}");    
        
    app.MapControllerRoute(
        name: "MyArea",
        pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

    app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");

    await SeedDatabaseAsync(app);

    await SeedLanguageAsync(app);

    await SeedProductsAsync(app);

    app.Run("http://localhost:5037");
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application start-up failed");
}
finally
{
    Log.CloseAndFlush();
}

static async Task SeedLanguageAsync(WebApplication app)
{
    using var scope = app.Services.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<LocalizationDbContext>();
    context.Database.EnsureCreated();

    var service = scope.ServiceProvider.GetRequiredService<SeedLanguage>();
    await SeedLanguage.EnsureSeedLanguageAsync();

}

//Seed Data
static async Task SeedDatabaseAsync(WebApplication app)
{
    using var scope = app.Services.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    context.Database.EnsureCreated();

    var service = scope.ServiceProvider.GetRequiredService<SeedData>();
    await SeedData.EnsureSeedDataAsync();
}
//Seed Products
static async Task SeedProductsAsync(WebApplication app)
{
    using var scope = app.Services.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<CatalogDbContext>();
    context.Database.EnsureCreated();

    var service = scope.ServiceProvider.GetRequiredService<SeedProducts>();
    await SeedProducts.EnsureSeedDataAsync();
}