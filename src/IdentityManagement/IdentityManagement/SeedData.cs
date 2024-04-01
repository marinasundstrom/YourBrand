using System.Security.Claims;
using IdentityModel;
using YourBrand.IdentityManagement.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;
using YourBrand.IdentityManagement.Infrastructure.Persistence;

namespace YourBrand.IdentityManagement;

public static class SeedData
{
    public static async Task EnsureSeedData(this WebApplication app)
    {
        using (var scope = app.Services.GetRequiredService<IServiceScopeFactory>().CreateScope())
        {
            var context = scope.ServiceProvider.GetService<ApplicationDbContext>();
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();

            var tenant = new Tenant(AcmeTenant.TenantId, "ACME Inc", null);

            context.Tenants.Add(tenant);

            context.SaveChanges();

            var organization = new Organization("ACME Testville", null) 
            { 
                Tenant = tenant 
            };

            context.Organizations.Add(organization);

            context.SaveChanges();

            var roleMgr = scope.ServiceProvider.GetRequiredService<RoleManager<Role>>();
            await roleMgr.CreateAsync(new Role("User"));
            await roleMgr.CreateAsync(new Role("Administrator"));

            var userMgr = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
            var alice = userMgr.FindByNameAsync("alice").Result;
            if (alice == null)
            {
                alice = new User
                {
                    FirstName  = "Alice",
                    LastName = "Smith",
                    UserName = "alice",
                    Email = "AliceSmith@email.com",
                    EmailConfirmed = true,
                    Tenant = tenant,
                    //Organization = organization
                };

                organization.AddUser(alice);

                var result = userMgr.CreateAsync(alice, "Pass123$").Result;
                if (!result.Succeeded)
                {
                    throw new Exception(result.Errors.First().Description);
                }

                result = userMgr.AddClaimsAsync(alice, [
                            new Claim(JwtClaimTypes.Name, "Alice Smith"),
                            new Claim(JwtClaimTypes.GivenName, "Alice"),
                            new Claim(JwtClaimTypes.FamilyName, "Smith"),
                            new Claim(JwtClaimTypes.WebSite, "http://alice.com"),
                            new Claim("tenant_id", alice.Tenant.Id)
                        ]).Result;
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
                    UserName = "bob",
                    Email = "BobSmith@email.com",
                    EmailConfirmed = true,
                    Tenant = tenant,
                    //Organization = organization
                };

                organization.AddUser(bob);

                var result = userMgr.CreateAsync(bob, "Pass123$").Result;
                if (!result.Succeeded)
                {
                    throw new Exception(result.Errors.First().Description);
                }

                result = userMgr.AddClaimsAsync(bob, [
                            new Claim(JwtClaimTypes.Name, "Bob Smith"),
                            new Claim(JwtClaimTypes.GivenName, "Bob"),
                            new Claim(JwtClaimTypes.FamilyName, "Smith"),
                            new Claim(JwtClaimTypes.WebSite, "http://bob.com"),
                            new Claim("tenant_id", bob.Tenant.Id)
                        ]).Result;
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
}
