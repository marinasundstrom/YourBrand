using YourBrand.ChatApp.Domain.ValueObjects;

namespace YourBrand.ChatApp.Features.Chat.Messages;

public class PostMessageRequest
{
    public string ChannelId { get; set; }

    public string? ReplyToId { get; set; }

    public string Content { get; set; } = default!;
}