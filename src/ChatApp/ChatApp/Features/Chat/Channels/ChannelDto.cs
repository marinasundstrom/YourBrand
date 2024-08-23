using YourBrand.ChatApp.Features.Users;

namespace YourBrand.ChatApp.Features.Chat.Channels;

public sealed record ChannelDto(Guid Id, string Name);

public sealed record ParticipantDto(Guid Id, Guid ChannelId, string Name, string? UserId);
