using ChatApp.Domain.ValueObjects;

namespace ChatApp.Domain.Entities;

public sealed class Channel : AggregateRoot<ChannelId>, IAuditable
{
    private Channel() : base(new ChannelId())
    {
    }

    public Channel(string title)
        : base(new ChannelId())
    {
        Title = title;

        // Todo: Emit Domain Event
    }

    internal Channel(ChannelId id, string title)
        : base(id)
    {
        Title = title;
    }

    public string Title { get; private set; } = null!;

    public bool Rename(string newTitle) 
    {
        var oldTitle = Title;

        if(newTitle == Title) 
            return false;

        Title = newTitle;

        // Todo: Emit Domain Event

        AddDomainEvent(new ChannelRenamed(Id, newTitle, oldTitle));
        return true;
    }

    public bool BlockPosting { get; set; }

    public bool DisallowNicknames { get; set; } 

    HashSet<ChannelParticipant> _participants = new HashSet<ChannelParticipant>();

    public IReadOnlyCollection<ChannelParticipant> Participants => _participants;

    public bool AddParticipant(UserId userId) 
    {
        var participant = Participants.First(x => x.UserId == userId);

        if(participant is not null) return false;

        _participants.Add(new ChannelParticipant(userId, DateTimeOffset.UtcNow));

        AddDomainEvent(new ParticipantAddedToChannel(Id, userId));

        return true;
    }

    public bool RemoveParticipant(UserId userId) 
    {
        var participant = Participants.First(x => x.UserId == userId);

        if(participant is null) return false;

        participant.Left = DateTimeOffset.UtcNow;
        _participants.Remove(participant);

        AddDomainEvent(new ParticipantRemovedFromChannel(Id, userId));

        return true;
    }

    public UserId? CreatedById { get; set; } = null!;
    public DateTimeOffset Created { get; set; }

    public UserId? LastModifiedById { get; set; }
    public DateTimeOffset? LastModified { get; set; }
}

public class ChannelParticipant
{
    private ChannelParticipant()
    {
    }

    public ChannelParticipant(UserId userId, DateTimeOffset joined)
    {
        UserId = userId;
        Joined = joined;
    }

    public UserId UserId { get; set; }

    public string? Nickname { get; set; }

    public DateTimeOffset Joined { get; set; }

    public DateTimeOffset? Left { get; set; }

    public int? UnreadMessages { get; set; }

    public bool IsMuted { get; set; }

    public DateTime? MutedUntil { get; set; }

    public DateTimeOffset? Added { get; set; }

    public UserId? AddedById { get; set; }

    public DateTimeOffset? Removed { get; set; }

    public UserId? RemovedById { get; set; }

    public DateTimeOffset? Blocked { get; set; }

    public UserId? BlockedById { get; set; }
}