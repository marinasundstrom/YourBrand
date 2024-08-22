using YourBrand.ChatApp.Domain.ValueObjects;

namespace YourBrand.ChatApp.Domain.Entities;

public interface IAuditableMessage
{
    ChannelParticipantId? PostedById { get; set; }
    DateTimeOffset Posted { get; set; }

    ChannelParticipantId? EditedById { get; set; }
    DateTimeOffset? Edited { get; set; }

    ChannelParticipantId? DeletedById { get; set; }
    DateTimeOffset? Deleted { get; set; }
}