namespace YourBrand.ChatApp.Features.Chat;

public static class WebApplicationExtensions
{
    public static WebApplication MapChatHubs(this WebApplication app)
    {
        app.MapHub<ChatHub>("/hubs/chat");

        return app;
    }
}
