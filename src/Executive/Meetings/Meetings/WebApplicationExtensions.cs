using YourBrand.Meetings.Features.Procedure;
using YourBrand.Meetings.Features.Procedure.Discussions;
using YourBrand.Meetings.Features.Minutes;

//using YourBrand.Meetings.Meetings;

namespace YourBrand.Meetings;

public static class WebApplicationExtensions
{
    public static WebApplication MapHubsForApp(this WebApplication app)
    {
        app.MapHub<MeetingsProcedureHub>("/hubs/meetings/procedure", options =>
        {
            options.EnableAuthenticationRefresh = true;
            options.CloseOnAuthenticationExpiration = true;
        });
        app.MapHub<SecretaryHub>("/hubs/meetings/secretary", options =>
        {
            options.EnableAuthenticationRefresh = true;
            options.CloseOnAuthenticationExpiration = true;
        });
        //app.MapHub<DiscussionsHub>("/hubs/meetings/discussions");

        return app;
    }
}
