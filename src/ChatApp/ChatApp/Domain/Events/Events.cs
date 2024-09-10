using YourBrand.ChatApp.Domain.ValueObjects;
using YourBrand.Domain;
using YourBrand.Tenancy;

namespace YourBrand.ChatApp.Domain.Events;

public sealed record ChannelRenamed(TenantId TenantId, ChannelId ChannelId, string NewName, string OldName) : DomainEvent;

public sealed record MessagePosted(TenantId TenantId, ChannelId ChannelId, MessageId MessageId, MessageId? ReplyToId) : DomainEvent;

public sealed record MessageEdited(TenantId TenantId, ChannelId ChannelId, MessageId MessageId, string Content) : DomainEvent;

public sealed record MessageDeleted(TenantId TenantId, ChannelId ChannelId, MessageId MessageId) : DomainEvent;

public sealed record UserReactedToMessage(TenantId TenantId, ChannelId ChannelId, MessageId MessageId, ChannelParticipantId ParticipantId, string Reaction) : DomainEvent;

public sealed record UserRemovedReactionFromMessage(TenantId TenantId, ChannelId ChannelId, MessageId MessageId, ChannelParticipantId ParticipantId, string Reaction) : DomainEvent;

public sealed record ParticipantAddedToChannel(TenantId TenantId, ChannelId ChannelId, ChannelParticipantId ParticipantId) : DomainEvent;

public sealed record ParticipantRemovedFromChannel(TenantId TenantId, ChannelId ChannelId, ChannelParticipantId ParticipantId) : DomainEvent;

public sealed record UserJoinedChannel(TenantId TenantId, ChannelId ChannelId, ChannelParticipantId ParticipantId) : DomainEvent;

public sealed record UserLeftChannel(TenantId TenantId, ChannelId ChannelId, ChannelParticipantId ParticipantId) : DomainEvent;