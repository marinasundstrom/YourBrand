using Microsoft.AspNetCore.Builder;

using YourBrand.Meetings.Features.Procedure.Command;

//using YourBrand.Meetings.Meetings;

namespace YourBrand.Meetings;

public static class WebApplicationExtensions
{
    public static WebApplication MapHubsForApp(this WebApplication app)
    {
        app.MapHub<MeetingsProcedureHub>("/hubs/meetings/procedure");

        return app;
    }
}