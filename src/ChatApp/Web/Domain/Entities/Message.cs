using ChatApp.Domain.ValueObjects;

namespace ChatApp.Domain.Entities;

public sealed class Message : AggregateRoot<MessageId>, IAuditable, ISoftDelete
{
    private HashSet<MessageReaction> _reactions = new HashSet<MessageReaction>();

    private Message() : base(new MessageId())
    {
    }

    public Message(ChannelId channelId, string content)
        : base(new MessageId())
    {
        ChannelId = channelId;
        Content = content;

        AddDomainEvent(new MessagePosted(ChannelId, Id, null));
    }

    public Message(ChannelId channelId, MessageId replyToId, string content)
        : base(new MessageId())
    {
        ChannelId = channelId;
        Content = content;
        
        ReplyToId = replyToId;

        AddDomainEvent(new MessagePosted(ChannelId, Id, ReplyToId));
    }

    public MessageId? ReplyToId { get; set; }

    public string Content { get; private set; } = null!;

    public bool UpdateContent(string newContent) 
    {
        if(newContent == Content) 
            return false;

        Content = newContent;

        AddDomainEvent(new MessageEdited(ChannelId, Id, Content));

        return true;
    }

    public IReadOnlyCollection<MessageReaction> Reactions => _reactions;

    public bool React(UserId userId, string reaction) 
    {
        var r = Reactions.FirstOrDefault(x => x.UserId == userId && x.Reaction == reaction);

        if(r is not null)
            return false;

        _reactions.Add(new MessageReaction(userId, reaction));

        AddDomainEvent(new UserReactedToMessage(ChannelId, Id, userId, reaction));

        return true;
    }

    public bool RemoveReaction(UserId userId, string reaction) 
    {
        var r = Reactions.FirstOrDefault(x => x.UserId == userId && x.Reaction == reaction);

        if(r is null)
            return false;
            
        _reactions.Remove(r);

        AddDomainEvent(new UserRemovedReactionFromMessage(ChannelId, Id, userId, r.Reaction));

        return true;
    }

    public void RemoveAllReactions() 
    {
        _reactions.Clear();
    }

    public void MarkAsDeleted()
    {
        UpdateContent(string.Empty);

        AddDomainEvent(new MessageDeleted(ChannelId, Id));
    }

    public DateTimeOffset Published => Created;

    public ChannelId ChannelId { get; private set; }

    public UserId? CreatedById { get; set; } = null!;
    public DateTimeOffset Created { get; set; }

    public UserId? LastModifiedById { get; set; }
    public DateTimeOffset? LastModified { get; set; }

    public UserId? DeletedById { get; set; }
    public DateTimeOffset? Deleted { get; set; }
}

public class MessageReaction
{
    public MessageReaction(UserId userId, string reaction)
    {
        UserId = userId;
        Reaction = reaction;
        Date = DateTime.UtcNow;
    }

    public UserId UserId { get; private set; }
    public string Reaction { get; private set; }
    public DateTime Date { get; private set; }
}