using Microsoft.AspNetCore.Builder;

using YourBrand.Meetings.Features.Procedure;
using YourBrand.Meetings.Features.Procedure.Command;
using YourBrand.Meetings.Features.Procedure.Discussions;

//using YourBrand.Meetings.Meetings;

namespace YourBrand.Meetings;

public static class WebApplicationExtensions
{
    public static WebApplication MapHubsForApp(this WebApplication app)
    {
        app.MapHub<MeetingsProcedureHub>("/hubs/meetings/procedure");
        app.MapHub<DiscussionsHub>("/hubs/meetings/discussions");

        return app;
    }
}