using YourBrand.ChatApp.Domain.ValueObjects;

namespace YourBrand.ChatApp.Domain.Entities;

public interface IAuditableMessage
{
    ChannelParticipantId? PostedById { get; set; }
    DateTimeOffset Posted { get; set; }

    ChannelParticipantId? LastEditedById { get; set; }
    DateTimeOffset? LastEdited { get; set; }

    ChannelParticipantId? DeletedById { get; set; }
    DateTimeOffset? Deleted { get; set; }
}