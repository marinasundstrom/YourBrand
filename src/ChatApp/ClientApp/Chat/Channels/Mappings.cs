using ChatApp;

namespace ChatApp.Chat.Channels;

public static class Mappings
{
    public static Message Map(this MessageData message)
    {
        return new Message
        {
            Id = message.Id,
            ChannelId = message.ChannelId,
            ReplyTo = message.ReplyTo?.Map(),
            Content = message.Content,
            Published = message.Published,
            PublishedBy = message.PublishedBy.Map(),
            LastEdited = message.LastEdited,
            LastEditedBy = message.LastEditedBy?.Map(),
            Deleted = message.Deleted,
            DeletedBy = message.DeletedBy?.Map(),
            Reactions = message.Reactions.Select(x => x.Map()).ToList()
        };
    }

    public static ReplyMessage Map(this ReplyMessageData message)
    {
        return new ReplyMessage
        {
            Id = message.Id,
            ChannelId = message.ChannelId,
            Content = message.Content,
            Published = message.Published,
            PublishedBy = message.PublishedBy.Map(),
            Deleted = message.Deleted,
            DeletedBy = message.DeletedBy?.Map(),
        };
    }

    public static User Map(this UserData user)
    {
        return new User
        {
            Id = user.Id,
            Name = user.Name
        };
    }

    public static Reaction Map(this MessageReactionData reaction)
    {
        return new Reaction
        {
            Content = reaction.Content,
            Date = reaction.Date,
            User = reaction.User.Map()
        };
    }
}