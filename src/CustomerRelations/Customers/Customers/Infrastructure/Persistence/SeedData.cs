﻿using YourBrand.Customers.Domain.Entities;
using YourBrand.Tenancy;

namespace YourBrand.Customers.Infrastructure.Persistence;

public class SeedData
{
    public static async Task EnsureSeedData(WebApplication app, string tenantId)
    {
        using (var scope = app.Services.GetRequiredService<IServiceScopeFactory>().CreateScope())
        {
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<SeedData>>();

            var tenantContext = scope.ServiceProvider.GetRequiredService<ISettableTenantContext>();
            tenantContext.SetTenantId(string.IsNullOrWhiteSpace(tenantId) ? TenantConstants.TenantId : tenantId);

            var context = scope.ServiceProvider.GetRequiredService<CustomersContext>();

            await context.Database.EnsureDeletedAsync();
            await context.Database.EnsureCreatedAsync();

            if (!context.Persons.Any())
            {
                var person = new Person("John", "Doe", "3234234234")
                {
                    Name = "John Doe",
                    Phone = null,
                    PhoneMobile = "072423123",
                    Email = "john.d@email.com",
                };

                person.AddAddress(new Address
                {
                    Type = Domain.Enums.AddressType.Billing,
                    Thoroughfare = "Baker Street",
                    SubPremises = null,
                    Premises = "42",
                    PostalCode = "4534 23",
                    Locality = "Testville",
                    SubAdministrativeArea = "Sub",
                    AdministrativeArea = "Area",
                    Country = "Testland"
                });

                context.Persons.Add(person);

                await context.SaveChangesAsync();
            }

            if (!context.Organizations.Any())
            {
                var organization = new Organization("John", "Doe", "3234234234")
                {
                    Name = "ACME Inc.",
                    OrganizationNo = "2323434",
                    VatNo = "SE-2323434",
                    Phone = null,
                    PhoneMobile = "072423123",
                    Email = "acme@email.com",
                };

                organization.AddAddress(new Address
                {
                    Type = Domain.Enums.AddressType.Billing,
                    Thoroughfare = "Baker Street",
                    SubPremises = null,
                    Premises = "42",
                    PostalCode = "4534 23",
                    Locality = "Testville",
                    SubAdministrativeArea = "Sub",
                    AdministrativeArea = "Area",
                    Country = "Testland"
                });

                context.Organizations.Add(organization);

                await context.SaveChangesAsync();
            }
        }
    }
}