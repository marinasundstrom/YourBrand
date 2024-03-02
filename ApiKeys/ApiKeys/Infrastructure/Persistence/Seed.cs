
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
            context.Users.Add(new User
            {
                Id = "api",
                FirstName = "API",
                LastName = "User",
                Email = "test@foo.com",
                Hidden = true
            });

            await context.SaveChangesAsync();
        }

        if (!context.ApiKeys.Any())
        {
            var service = new Service
            {
                Id = Guid.NewGuid().ToString(),
                Name = "IdentityService",
                Url = "https://localhost:5040/",
                Secret = "dfEKJnHVikyDT9em6QQZAg"
            };

            context.Services.Add(service);

            var service2 = new Service
            {
                Id = Guid.NewGuid().ToString(),
                Name = "TimeReport",
                Url = "https://localhost/api/timereport/",
                Secret = "r9qkLo5BikSwPdhMKNx4EA"
            };

            context.Services.Add(service2);

            var service3 = new Service
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Showroom",
                Url = "https://localhost/api/showroom/",
                Secret = "ai9JxJ0Cck+VQPSeoLewlQ"
            };

            context.Services.Add(service3);

            var application = new Domain.Entities.Application
            {
                Id = Guid.NewGuid().ToString(),
                Name = "YourBrand"
            };

            context.Applications.Add(application);

            var apiKey = new ApiKey
            {
                Id = "myKey",
                Application = application,
                Key = "asdsr34#34rswert35234aedae?2!",
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