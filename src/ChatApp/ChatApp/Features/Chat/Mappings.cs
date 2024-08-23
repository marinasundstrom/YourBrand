using YourBrand.ChatApp;
using YourBrand.ChatApp.Features.Chat.Channels;
using YourBrand.ChatApp.Features.Users;

namespace YourBrand.ChatApp.Features.Chat;

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
            message.Posted,
            message.PostedBy.Map(),
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
            message.Posted,
            message.PostedBy.Map(),
            message.LastEdited,
            message.LastEditedBy?.Map(),
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

    public static ParticipantData Map(this ParticipantDto participant)
    {
        return new ParticipantData
        (
            participant.Id.ToString(),
            participant.Name,
            participant.UserId
        );
    }

    public static MessageReactionData Map(this ReactionDto reaction)
    {
        return new MessageReactionData
        (
            reaction.Content,
            reaction.Date,
            reaction.AddedBy.Map()
        );
    }
}