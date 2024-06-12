namespace ChatApp.Features.Chat;

public static class WebApplicationExtensions
{
    public static WebApplication MapTodoHubs(this WebApplication app)
    {
        app.MapHub<ChatHub>("/hubs/chat");

        return app;
    }
}
