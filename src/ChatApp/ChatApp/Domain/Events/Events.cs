using YourBrand.ChatApp.Domain.ValueObjects;
using YourBrand.Domain;

namespace YourBrand.ChatApp.Domain.Events;

public sealed record ChannelRenamed(ChannelId ChannelId, string NewName, string OldName) : DomainEvent;

public sealed record MessagePosted(ChannelId ChannelId, MessageId MessageId, MessageId? ReplyToId) : DomainEvent;

public sealed record MessageEdited(ChannelId ChannelId, MessageId MessageId, string Content) : DomainEvent;

public sealed record MessageDeleted(ChannelId ChannelId, MessageId MessageId) : DomainEvent;

public sealed record UserReactedToMessage(ChannelId ChannelId, MessageId MessageId, ChannelParticipantId ParticipantId, string Reaction) : DomainEvent;

public sealed record UserRemovedReactionFromMessage(ChannelId ChannelId, MessageId MessageId, ChannelParticipantId ParticipantId, string Reaction) : DomainEvent;

public sealed record ParticipantAddedToChannel(ChannelId ChannelId, ChannelParticipantId ParticipantId) : DomainEvent;

public sealed record ParticipantRemovedFromChannel(ChannelId ChannelId, ChannelParticipantId ParticipantId) : DomainEvent;

public sealed record UserJoinedChannel(ChannelId ChannelId, ChannelParticipantId ParticipantId) : DomainEvent;

public sealed record UserLeftChannel(ChannelId ChannelId, ChannelParticipantId ParticipantId) : DomainEvent;