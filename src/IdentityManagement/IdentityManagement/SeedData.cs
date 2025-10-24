using System.Security.Claims;

using IdentityModel;

using Microsoft.AspNetCore.Identity;

using Serilog;

using YourBrand.IdentityManagement.Domain.Entities;
using YourBrand.IdentityManagement.Infrastructure.Persistence;
using YourBrand.Tenancy;

namespace YourBrand.IdentityManagement;

public static class SeedData
{
    public static async Task EnsureSeedData(this WebApplication app, string tenantId)
    {
        using (var scope = app.Services.GetRequiredService<IServiceScopeFactory>().CreateScope())
        {
            var tenantIdValue = string.IsNullOrWhiteSpace(tenantId) ? TenantConstants.TenantId : tenantId;

            var tenantContext = scope.ServiceProvider.GetRequiredService<ISettableTenantContext>();
            tenantContext.SetTenantId(tenantIdValue);

            using var context = scope.ServiceProvider.GetService<ApplicationDbContext>();

            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();

            var tenant = new Tenant(tenantIdValue, TenantConstants.TenantName, null);

            context.Tenants.Add(tenant);

            context.SaveChanges();

            var organization = new Organization(TenantConstants.OrganizationId, TenantConstants.OrganizationName, null)
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
                    Id = TenantConstants.UserAliceId,
                    FirstName = "Alice",
                    LastName = "Smith",
                    UserName = "alice",
                    Email = "AliceSmith@email.com",
                    EmailConfirmed = true,
                    //Tenant = tenant
                };

                tenant.AddUser(alice);

                organization.AddUser(alice);

                var result = userMgr.CreateAsync(alice, "Pass123$").Result;
                if (!result.Succeeded)
                {
                    throw new Exception(result.Errors.First().Description);
                }

                result = await userMgr.AddToRoleAsync(alice, "Administrator");

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
                    Id = TenantConstants.UserBobId,
                    FirstName = "Bob",
                    LastName = "Smith",
                    UserName = "bob",
                    Email = "BobSmith@email.com",
                    EmailConfirmed = true,
                    //Tenant = tenant
                };

                tenant.AddUser(bob);

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