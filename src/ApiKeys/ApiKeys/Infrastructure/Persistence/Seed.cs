
using YourBrand.ApiKeys.Domain.Entities;

namespace YourBrand.ApiKeys.Infrastructure.Persistence;

public static class Seed
{
    public static async Task SeedAsync(this IServiceProvider services)
    {
        using var scope = services.CreateScope();
        using var context = scope.ServiceProvider.GetRequiredService<ApiKeysContext>();

        await context.Database.EnsureDeletedAsync();
        await context.Database.EnsureCreatedAsync();

        if (!context.Users.Any())
        {
            context.Users.Add(new User("api")
            {
                FirstName = "API",
                LastName = "User",
                Email = "test@foo.com",
                Hidden = true
            });

            await context.SaveChangesAsync();
        }

        if (!context.ApiKeys.Any())
        {
            var service = new Service(
                "IdentityService",
                null,
                "https://localhost:5040/",
                "dfEKJnHVikyDT9em6QQZAg"
            );

            context.Services.Add(service);

            var service2 = new Service(
                "TimeReport",
                null,
                "https://localhost:5174/api/timereport/",
                "r9qkLo5BikSwPdhMKNx4EA"
            );

            context.Services.Add(service2);

            var service3 = new Service("Showroom", null, "https://localhost:5174/api/showroom/", "secret");

            context.Services.Add(service3);

            var application = new Domain.Entities.Application("YourBrand", null);

            context.Applications.Add(application);

            var apiKey = new ApiKey("asdsr34#34rswert35234aedae?2!", null)
            {
                Application = application,
            };

            apiKey.ApiKeyServices.Add(new ApiKeyService
            {
                Service = service
            });

            apiKey.ApiKeyServices.Add(new ApiKeyService
            {
                Service = service2
            });

            apiKey.ApiKeyServices.Add(new ApiKeyService
            {
                Service = service3
            });

            context.ApiKeys.Add(apiKey);

            await context.SaveChangesAsync();
        }
    }
}