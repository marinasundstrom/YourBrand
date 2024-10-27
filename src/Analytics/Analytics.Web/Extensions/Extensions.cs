using YourBrand.Extensions;

namespace YourBrand.Analytics.Web;

public static class Extensions
{
    public static IHostApplicationBuilder AddApplicationServices(this IHostApplicationBuilder builder)
    {
        //builder.AddDefaultAuthentication();
        builder.Services.AddAuthenticationServices(builder.Configuration);

        return builder;
    }
}
