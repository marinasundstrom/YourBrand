using YourBrand.ChatApp.Domain.ValueObjects;

namespace YourBrand.ChatApp.Domain.Entities;

public interface IChatAuditable
{
    ChannelParticipantId? PostedById { get; set; }
    DateTimeOffset Posted { get; set; }

    ChannelParticipantId? LastEditedById { get; set; }
    DateTimeOffset? LastEdited { get; set; }
}