using Microsoft.AspNetCore.Builder;

using YourBrand.Ticketing.Application.Features.Tickets;

namespace YourBrand.Ticketing.Application;

public static class WebApplicationExtensions
{
    public static WebApplication MapHubsForApp(this WebApplication app)
    {
        app.MapHub<TicketsHub>("/hubs/tickets", options =>
        {
            options.EnableAuthenticationRefresh = true;
            options.CloseOnAuthenticationExpiration = true;
        });

        return app;
    }
}