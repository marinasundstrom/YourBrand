using ChatApp.Features.Chat;
using ChatApp.Features.Chat.Channels;
using ChatApp.Features.Users;

namespace ChatApp;

public static class Mappings
{
    public static ChannelDto ToDto(this Channel channel) => new (channel.Id, channel.Title);

    public static MessageDto ToDto(this Message message) => new (message.Id, message.ChannelId, null, message.Content, message.Published, new UserDto(message.CreatedById.ToString()!, ""), message.LastModified, message.LastModifiedById is null ? null : new UserDto(message.LastModifiedById.ToString()!, ""), message.Deleted, null, null!);

    public static UserDto ToDto(this User user) => new (user.Id, user.Name);

    public static UserInfoDto ToDto2(this User user) => new (user.Id, user.Name);
}
