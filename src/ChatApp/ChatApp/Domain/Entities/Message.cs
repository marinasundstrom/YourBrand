using YourBrand.ChatApp.Domain.ValueObjects;
using YourBrand.Domain;

namespace YourBrand.ChatApp.Domain.Entities;

public sealed class Message : AggregateRoot<MessageId>, IAuditableMessage //, ISoftDelete
{
    private readonly HashSet<MessageReaction> _reactions = new HashSet<MessageReaction>();

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
        if (newContent == Content)
            return false;

        Content = newContent;

        AddDomainEvent(new MessageEdited(ChannelId, Id, Content));

        return true;
    }

    public IReadOnlyCollection<MessageReaction> Reactions => _reactions;

    public bool React(ChannelParticipantId participantId, string reaction)
    {
        var r = Reactions.FirstOrDefault(x => x.AddedById == participantId && x.Reaction == reaction);

        if (r is not null)
            return false;

        _reactions.Add(new MessageReaction(participantId, reaction, DateTimeOffset.UtcNow));

        AddDomainEvent(new UserReactedToMessage(ChannelId, Id, participantId, reaction));

        return true;
    }

    public bool RemoveReaction(ChannelParticipantId participantId, string reaction)
    {
        var r = Reactions.FirstOrDefault(x => x.AddedById == participantId && x.Reaction == reaction);

        if (r is null)
            return false;

        _reactions.Remove(r);

        AddDomainEvent(new UserRemovedReactionFromMessage(ChannelId, Id, participantId, r.Reaction));

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

    public ChannelId ChannelId { get; private set; }

    public ChannelParticipant? PostedBy { get; set; }
    public ChannelParticipantId? PostedById { get; set; }
    public DateTimeOffset Posted { get; set; }

    public ChannelParticipant? LastEditedBy { get; set; }
    public ChannelParticipantId? LastEditedById { get; set; }
    public DateTimeOffset? LastEdited { get; set; }

    public ChannelParticipant? DeletedBy { get; set; }
    public ChannelParticipantId? DeletedById { get; set; }
    public DateTimeOffset? Deleted { get; set; }
}

public class MessageReaction : IEntity
{
    public MessageReaction(ChannelParticipantId addedById, string reaction, DateTimeOffset date)
    {
        AddedById = addedById;
        Reaction = reaction;
        Date = date;
    }

    //public ChannelParticipant AddedBy { get; private set; }
    public ChannelParticipantId AddedById { get; set; }
    public string Reaction { get; set; }
    public DateTimeOffset Date { get; set; }
}