using YourBrand.Marketing.Domain.Entities;

namespace YourBrand.Marketing.Infrastructure.Persistence;

public class SeedData
{
    public static async Task EnsureSeedData(WebApplication app)
    {
        using (var scope = app.Services.GetRequiredService<IServiceScopeFactory>().CreateScope())
        {
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<SeedData>>();

            var context = scope.ServiceProvider.GetRequiredService<MarketingContext>();
            await context.Database.EnsureDeletedAsync();
            //context.Database.Migrate();
            await context.Database.EnsureCreatedAsync();

            if (!context.Contacts.Any())
            {
                var person = new Contact("John", "Doe", "3234234234")
                {
                    Phone = null,
                    PhoneMobile = "072423123",
                    Email = "test@test.com",
                    Address = new Domain.ValueObjects.Address(
                        Thoroughfare:"Baker Street",
                        SubPremises: null,
                        Premises: "42",
                        PostalCode: "4534 23",
                        Locality: "Testville",
                        SubAdministrativeArea: "Sub",
                        AdministrativeArea: "Area",
                        Country: "Testland"
                    )
                };

                context.Contacts.Add(person);

                await context.SaveChangesAsync();
            }
        }
    }
}