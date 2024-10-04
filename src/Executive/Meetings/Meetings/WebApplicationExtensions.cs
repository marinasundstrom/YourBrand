using Microsoft.AspNetCore.Builder;

//using YourBrand.Meetings.Meetings;

namespace YourBrand.Meetings;

public static class WebApplicationExtensions
{
    public static WebApplication MapHubsForApp(this WebApplication app)
    {
        //app.MapHub<TicketsHub>("/hubs/meetings");

        return app;
    }
}