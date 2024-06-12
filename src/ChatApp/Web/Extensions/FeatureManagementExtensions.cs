using Microsoft.FeatureManagement.FeatureFilters;

namespace ChatApp.Web.Extensions;

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
