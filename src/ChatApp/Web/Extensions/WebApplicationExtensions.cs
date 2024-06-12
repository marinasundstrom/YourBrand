using ChatApp.Features.Chat;
using ChatApp.Features.Chat.Channels;
using ChatApp.Features.Chat.Messages;
using ChatApp.Features.Users;

namespace ChatApp.Extensions;

public static class WebApplicationExtensions
{
    public static WebApplication MapApplicationEndpoints(this WebApplication app)
    {
        app.MapChannelEndpoints();
        app.MapMessageEndpoints();
        app.MapUsersEndpoints();

        return app;
    }

    public static WebApplication MapApplicationHubs(this WebApplication app)
    {
        app.MapTodoHubs();

        return app;
    }
}
