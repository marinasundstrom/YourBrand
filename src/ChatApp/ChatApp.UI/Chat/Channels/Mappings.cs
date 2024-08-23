using YourBrand.ChatApp;

namespace YourBrand.ChatApp.Chat.Channels;

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
            Posted = message.Posted,
            PostedBy = message.PostedBy.Map(),
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
            Posted = message.Posted,
            PostedBy = message.PostedBy.Map(),
            Deleted =  message.Deleted,
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

    public static Participant Map(this ParticipantData participant)
    {
        return new Participant
        {
            Id = Guid.Parse(participant.Id),
            Name = participant.Name,
            UserId = participant.UserId
        };
    }

    public static Reaction Map(this MessageReactionData reaction)
    {
        return new Reaction
        {
            Content = reaction.Content,
            Date = reaction.Date,
            AddedBy = reaction.AddedBy.Map()
        };
    }
}