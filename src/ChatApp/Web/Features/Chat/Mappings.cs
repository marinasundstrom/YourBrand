using ChatApp;
using ChatApp.Features.Users;

namespace ChatApp.Features.Chat;

public static class Mappings
{
    public static MessageData Map(this MessageDto message)
    {
        return new MessageData
        (
            message.Id,
            message.ChannelId,
            message.ReplyTo?.Map(),
            message.Content,
            message.Published,
            message.PublishedBy.Map(),
            message.LastEdited,
            message.LastEditedBy?.Map(),
            message.Deleted,
            message.DeletedBy?.Map(),
            message.Reactions.Select(x => x.Map()).ToList()
        );
    }

    public static ReplyMessageData Map(this ReplyMessageDto message)
    {
        return new ReplyMessageData
        (
            message.Id,
            message.ChannelId,
            message.Content,
            message.Published,
            message.PublishedBy.Map(),
            message.LastModified,
            message.LastModifiedBy?.Map(),
            message.Deleted,
            message.DeletedBy?.Map()
        );
    }

    public static UserData Map(this UserDto user)
    {
        return new UserData
        (
            user.Id,
            user.Name
        );
    }

    public static MessageReactionData Map(this ReactionDto reaction)
    {
        return new MessageReactionData
        (
            reaction.Content,
            reaction.Date,
            reaction.User.Map()
        );
    }
}