using MediatR;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Quartz;

using YourBrand.Analytics.Infrastructure.Persistence;
using YourBrand.Analytics.Infrastructure.Services;
using YourBrand.Domain.Infrastructure;

namespace YourBrand.Analytics.Infrastructure;

public static class ServiceExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddPersistence(configuration);

        services.AddScoped<IBlobStorageService, BlobStorageService>();

        services.AddDomainInfrastructure<ApplicationDbContext>(configuration);

        services.AddQuartz(configure =>
        {

        });

        services.AddQuartzHostedService(x => x.WaitForJobsToComplete = true);

        return services;
    }
}