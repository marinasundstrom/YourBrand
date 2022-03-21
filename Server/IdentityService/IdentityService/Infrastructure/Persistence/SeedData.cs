using System.Security.Claims;

using IdentityModel;

using Skynet.IdentityService.Domain.Entities;

using Microsoft.AspNetCore.Identity;

using Serilog;

namespace Skynet.IdentityService.Infrastructure.Persistence;

public class SeedData
{
    public static async Task EnsureSeedData(WebApplication app)
    {
        using (var scope = app.Services.GetRequiredService<IServiceScopeFactory>().CreateScope())
        {
            var context = scope.ServiceProvider.GetService<ApplicationDbContext>();
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();

            //await context.Database.MigrateAsync();

            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<Role>>();


            var result = await roleManager.CreateAsync(new Role()
            {
                Name = "Administrator"
            });

            if (!result.Succeeded)
            {
                throw new Exception(result.Errors.First().Description);
            }

            result = await roleManager.CreateAsync(new Role()
            {
                Name = "Manager"
            });

            if (!result.Succeeded)
            {
                throw new Exception(result.Errors.First().Description);
            }

            result = await roleManager.CreateAsync(new Role()
            {
                Name = "User"
            });

            if (!result.Succeeded)
            {
                throw new Exception(result.Errors.First().Description);
            }

            /*
            var userMgr = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
            var alice = userMgr.FindByNameAsync("alice").Result;
            if (alice == null)
            {
                alice = new User
                {
                    FirstName = "Alice",
                    LastName = "Smith",
                    UserName = "alice",
                    Email = "AliceSmith@email.com",
                    EmailConfirmed = true,
                };
                var result = userMgr.CreateAsync(alice, "Pass123$").Result;
                if (!result.Succeeded)
                {
                    throw new Exception(result.Errors.First().Description);
                }

                result = userMgr.AddClaimsAsync(alice, new Claim[]{
                            new Claim(JwtClaimTypes.Name, "Alice Smith"),
                            new Claim(JwtClaimTypes.GivenName, "Alice"),
                            new Claim(JwtClaimTypes.FamilyName, "Smith"),
                            new Claim(JwtClaimTypes.WebSite, "http://alice.com"),
                        }).Result;
                if (!result.Succeeded)
                {
                    throw new Exception(result.Errors.First().Description);
                }
                Log.Debug("alice created");
            }
            else
            {
                Log.Debug("alice already exists");
            }

            var bob = userMgr.FindByNameAsync("bob").Result;
            if (bob == null)
            {
                bob = new User
                {
                    FirstName = "Bob",
                    LastName = "Smith",
                    DisplayName = "Bob Smith",
                    UserName = "bob",
                    Email = "BobSmith@email.com",
                    EmailConfirmed = true
                };
                var result = userMgr.CreateAsync(bob, "Pass123$").Result;

                result = userMgr.AddClaimsAsync(bob, new Claim[]{
                            new Claim(JwtClaimTypes.Name, "Bob Smith"),
                            new Claim(JwtClaimTypes.GivenName, "Bob"),
                            new Claim(JwtClaimTypes.FamilyName, "Smith"),
                            new Claim(JwtClaimTypes.WebSite, "http://bob.com"),
                            new Claim("location", "somewhere")
                        }).Result;
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

            */
        }
    }
}