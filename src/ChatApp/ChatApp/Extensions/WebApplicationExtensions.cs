using YourBrand.ChatApp.Features.Chat;
using YourBrand.ChatApp.Features.Chat.Channels;
using YourBrand.ChatApp.Features.Chat.Messages;
using YourBrand.ChatApp.Features.Users;

namespace YourBrand.ChatApp.Extensions;

public static class WebApplicationExtensions
{
    public static WebApplication MapApplicationEndpoints(this WebApplication app)
    {
        app.MapChannelEndpoints();
        app.MapMessageEndpoints();

        return app;
    }

    public static WebApplication MapApplicationHubs(this WebApplication app)
    {
        app.MapChatHubs();

        return app;
    }
}