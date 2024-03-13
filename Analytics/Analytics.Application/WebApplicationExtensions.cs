using Microsoft.AspNetCore.Builder;
using YourBrand.Analytics.Application.Hubs;

namespace YourBrand.Analytics.Application;

public static class WebApplicationExtensions
{
    public static WebApplication MapHubsForApp(this WebApplication app)
    {
        app.MapHub<TodosHub>("/hubs/todos");

        return app;
    }
}
