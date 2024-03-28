using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.FeatureManagement.FeatureFilters;

namespace YourBrand.Extensions;

public static class FeatureManagementExtensions
{
    public static IServiceCollection AddFeatureManagement(this IServiceCollection services)
    {
        Microsoft.FeatureManagement.ServiceCollectionExtensions.AddFeatureManagement(services)
                .AddFeatureFilter<PercentageFilter>()
                .AddFeatureFilter<TimeWindowFilter>();

        return services;
    }
}