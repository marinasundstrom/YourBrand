using Asp.Versioning;

namespace ChatApp.Web.Extensions;

public static class ApiVersioningExtensions
{
    public static IServiceCollection AddApiVersioningServices(this IServiceCollection services)
    {
        services.AddApiVersioning(options =>
        {
            options.AssumeDefaultVersionWhenUnspecified = true;
            options.DefaultApiVersion = new ApiVersion(1, 0);
            options.ReportApiVersions = true;
            options.ApiVersionReader = new UrlSegmentApiVersionReader();
        })
       .AddApiExplorer(option =>
        {
            option.GroupNameFormat = "VVV";
            option.SubstituteApiVersionInUrl = true;
        })
        .EnableApiVersionBinding();

        return services;
    }
}
