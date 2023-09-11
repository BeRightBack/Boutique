using System.Security.Cryptography;
using System;
using System.Reflection;
using System.Security.Claims;
using Boutique.Data;
using Boutique.Entity;
using Microsoft.AspNetCore.Identity;
using Serilog;
using System.Linq;
using System.Threading.Tasks;

namespace Boutique.Configuration;


public class SeedData_example
{
    private static ApplicationDbContext _context;
    private static UserManager<ApplicationUser> _userManager;
    private static RoleManager<IdentityRole> _roleManager;


    public SeedData_example(ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        _context = context;
        _userManager = userManager;
        _roleManager = roleManager;
    }
    public static Task EnsureSeedDataAsync()
    {
        
        if (_context.Database == null)
        {
            throw new Exception("DB is null");
        }

        if (!_context.Roles.Any())
        {
            var member = _roleManager.FindByNameAsync("User").Result;
            var admin = _roleManager.FindByNameAsync("Administrator").Result;



            if (member == null)
            {
                member = new IdentityRole
                {
                    Name = "User"
                };
                _ = _roleManager.CreateAsync(member).Result;
            }


            if (admin == null)
            {
                admin = new IdentityRole
                {
                    Name = "Administrator"
                };
                _ = _roleManager.CreateAsync(admin).Result;
            }


            if (!_context.Users.Any())
            {
                var _BeRightBack = _userManager.FindByNameAsync("BeRightBack").Result;
                if (_BeRightBack == null)
                {
                    _BeRightBack = new ApplicationUser
                    {
                        FirstName = "Steven",
                        LastName = "Pinel",
                        UserName = "BeRightBack",
                        Email = "steven_pinel@example.com",
                        EmailConfirmed = true,
                        ProfilePicturePath = "https://example.com/Images/user-icon.jpg"
                    };
                    var result = _userManager.CreateAsync(_BeRightBack, "Password@2022").Result;
                    if (!result.Succeeded)
                    {
                        throw new Exception(result.Errors.First().Description);
                    }

                    result = _userManager.AddClaimsAsync(_BeRightBack, new Claim[]{
                       new Claim("user.username","BeRightBack"),
                       new Claim("user.firstname","Steven"),
                       new Claim("user.lastname","Pinel"),
                       new Claim("user.fullname", "Steven Pinel"),
                       new Claim("user.email","steven_pinel@example.com"),
                       new Claim("user.picture","https://example.com/Images/user-icon.jpg"),
                       new Claim("user.role","User"),
                       new Claim("user.website","https://example.com")
                    }).Result;

                    if (!_userManager.IsInRoleAsync(_BeRightBack, member.Name).Result)
                    {
                        _ = _userManager.AddToRoleAsync(_BeRightBack, member.Name).Result;
                    }

                    if (!result.Succeeded)
                    {
                        throw new Exception(result.Errors.First().Description);
                    }
                    Log.Debug("BeRightBack created");
                }
                else
                {
                    Log.Debug("BeRightBack already exists");
                }

                var bob = _userManager.FindByNameAsync("Administrator").Result;
                if (bob == null)
                {
                    bob = new ApplicationUser
                    {
                        FirstName = "Steven",
                        LastName = "Pinel",
                        UserName = "Administrator",
                        Email = "admin@example.com",
                        EmailConfirmed = true,
                        ProfilePicturePath = "https://example.com/Images/user-icon.jpg"
                    };
                    var result = _userManager.CreateAsync(bob, "Password@2022").Result;
                    if (!result.Succeeded)
                    {
                        throw new Exception(result.Errors.First().Description);
                    }

                    result = _userManager.AddClaimsAsync(bob, new Claim[]{
                       new Claim("user.username","Administrator"),
                       new Claim("user.firstname","Steven"),
                       new Claim("user.lastname","Pinel"),
                       new Claim("user.fullname", "Steven Pinel"),
                       new Claim("user.email","admin@example.com"),
                       new Claim("user.picture","https://example.com/Images/user-icon.jpg"),
                       new Claim("user.role","User"),
                       new Claim("user.role","Administrator"),
                       new Claim("user.website","https://example.com")
                    }).Result;

                    if (!_userManager.IsInRoleAsync(bob, admin.Name).Result)
                    {
                        _ = _userManager.AddToRoleAsync(bob, admin.Name).Result;
                    }


                    if (!result.Succeeded)
                    {
                        throw new Exception(result.Errors.First().Description);
                    }
                    Log.Debug("bob created");
                }
                else
                {
                    Log.Debug("bob already exists");
                }
            }
        }

        return Task.CompletedTask;
    }
}



