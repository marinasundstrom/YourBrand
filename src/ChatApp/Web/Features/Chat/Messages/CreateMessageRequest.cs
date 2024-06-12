namespace ChatApp.Features.Chat.Messages;

public class PostMessageRequest
{
    public Guid ChannelId { get; set; }

    public Guid? ReplyToId { get; set; }

    public string Content { get; set; } = default!;
}
