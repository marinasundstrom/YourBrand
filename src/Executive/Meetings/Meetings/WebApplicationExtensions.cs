using YourBrand.Meetings.Features.Procedure;
using YourBrand.Meetings.Features.Procedure.Discussions;
using YourBrand.Meetings.Features.Minutes;

//using YourBrand.Meetings.Meetings;

namespace YourBrand.Meetings;

public static class WebApplicationExtensions
{
    public static WebApplication MapHubsForApp(this WebApplication app)
    {
        app.MapHub<MeetingsProcedureHub>("/hubs/meetings/procedure");
        app.MapHub<SecretaryHub>("/hubs/meetings/secretary");
        //app.MapHub<DiscussionsHub>("/hubs/meetings/discussions");

        return app;
    }
}
