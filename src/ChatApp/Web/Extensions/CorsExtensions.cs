namespace ChatApp.Web.Extensions;

public static class CorsExtensions
{
    public readonly static string MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

    public static IServiceCollection AddCorsService(this IServiceCollection services)
    {
        services.AddCors(options =>
        {
            options.AddPolicy(name: MyAllowSpecificOrigins,
                            policy =>
                            {
                                policy.WithOrigins("https://localhost:5021")
                                .AllowAnyHeader().AllowAnyMethod();
                            });
        });

        return services;
    }
}
