namespace YourBrand.ChatApp.Features.Chat;

public interface IChatHub
{
    Task<string> PostMessage(string channelId, string? replyTo, string content);
}