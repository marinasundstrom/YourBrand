namespace ChatApp.Features.Chat;

public interface IChatHub
{
    Task<Guid> PostMessage(Guid channelId, Guid? replyTo, string content);
}
