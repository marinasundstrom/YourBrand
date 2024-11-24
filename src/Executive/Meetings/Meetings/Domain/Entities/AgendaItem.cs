using YourBrand.Auditability;
using YourBrand.Domain;
using YourBrand.Identity;
using YourBrand.Meetings.Domain.ValueObjects;
using YourBrand.Tenancy;

namespace YourBrand.Meetings.Domain.Entities;

public enum AgendaItemState
{
    Pending,
    UnderDiscussion,
    Voting,
    Completed,
    Postponed,
    Skipped,
    Canceled
}

public enum DiscussionActions
{
    None,
    Required,
    Optional
}

public enum VoteActions
{
    None,
    Required,
    Optional
}

public enum PostponementType
{
    LaterInMeeting,
    NextMeeting,
    Indefinite
}

public class AgendaItem : Entity<AgendaItemId>, IAuditableEntity<AgendaItemId>, IHasTenant, IHasOrganization
{
    private HashSet<AgendaItem> _subItems = new HashSet<AgendaItem>();

    protected AgendaItem() {}

    public AgendaItem(OrganizationId organizationId, AgendaItemType type, string title, string description)
    : base(new AgendaItemId())
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Title is required.", nameof(title));

        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("Description is required.", nameof(description));

        OrganizationId = organizationId;
        Type = type;
        Title = title;
        Description = description;

        if(Type == AgendaItemType.Discussion) 
        {
            Discussion = new Discussion()
            {
                OrganizationId = OrganizationId,
                //AgendaId = 
            };
        }
        else if (Type == AgendaItemType.Election)
        {
            Election = new Election() 
            {
                OrganizationId = OrganizationId
            };
        }
    }

    public TenantId TenantId { get; set; }
    public OrganizationId OrganizationId { get; set; }
    public AgendaId AgendaId { get; set; }

    public AgendaItemId? ParentId { get; set; }

    public AgendaItemType Type { get; set; }

    public string Title { get; set; }
    public string Description { get; set; }
    public AgendaItemState State { get; set; } = AgendaItemState.Pending;
    public int Order { get; set; }

    public bool IsMandatory { get; set; }
    public DiscussionActions DiscussionActions { get; set; } = DiscussionActions.Optional;
    public VoteActions VoteActions { get; set; } = VoteActions.Optional;

    // Estimated time for item to be completed
    public TimeSpan? EstimatedTime { get; private set; }

    public bool IsDiscussionCompleted { get; private set; }
    public bool IsVoteCompleted { get; private set; }

    // public AgendaItemId DependsOnItem { get; set; }

    public Discussion? Discussion { get; set; }
    public Voting? Voting { get; set; }
    public Election? Election { get; set; }

    public DateTimeOffset? DiscussionStartedAt { get; private set; }
    public DateTimeOffset? DiscussionEndedAt { get; private set; }
    public DateTimeOffset? VotingStartedAt { get; private set; }
    public DateTimeOffset? VotingEndedAt { get; private set; }

    // For motions
    public MotionId? MotionId { get; set; }

    // For election
    public string? Position { get; set; }

    [Throws(typeof(InvalidOperationException))]
    public void StartDiscussion()
    {
        if (State != AgendaItemState.Pending && State != AgendaItemState.Postponed)
            throw new InvalidOperationException("Discussion can only be started when the agenda item is pending or postponed.");

        if (IsDiscussionCompleted)
            throw new InvalidOperationException("Discussion already completed.");

        // Initialize Discussion if it hasn't been created yet (in case there were no prior requests)
        Discussion ??= new Discussion { OrganizationId = OrganizationId };
        Discussion.StartSession();

        DiscussionStartedAt = DateTimeOffset.UtcNow;
        State = AgendaItemState.UnderDiscussion;
    }

    [Throws(typeof(InvalidOperationException))]
    public void EndDiscussion()
    {
        Discussion?.EndSession();

        IsDiscussionCompleted = true;
        DiscussionEndedAt = DateTimeOffset.UtcNow;
    }

    [Throws(typeof(InvalidOperationException))]
    public void RequestSpeakerSlot(MeetingAttendee attendee)
    {
        if (State != AgendaItemState.Pending)
            throw new InvalidOperationException("Speaker slots can only be requested when the item is pending.");

        // Create Discussion only if it doesn't exist
        Discussion ??= new Discussion { OrganizationId = OrganizationId };
        Discussion.AddSpeakerRequest(attendee);
    }

    [Throws(typeof(InvalidOperationException))]
    public void StartVoting()
    {
        if (State != AgendaItemState.UnderDiscussion)
            throw new InvalidOperationException("Cannot start voting.");

        if (Voting != null)
            throw new InvalidOperationException("Voting session already in progress.");

        Voting = new Voting(VotingType.SimpleMajority)
        {
            OrganizationId = OrganizationId,
            TenantId = TenantId
        };

        Voting.StartVoting();

        VotingStartedAt = DateTimeOffset.UtcNow;
        State = AgendaItemState.Voting;
    }

    [Throws(typeof(InvalidOperationException))]
    public void EndVoting()
    {
        if (Voting == null)
            throw new InvalidOperationException("No active voting session.");

        Voting.TallyVotes();

        Voting.EndVoting();

        VotingEndedAt = DateTimeOffset.UtcNow;
        IsVoteCompleted = true;
        State = AgendaItemState.Completed;
    }

    [Throws(typeof(InvalidOperationException))]
    public void StartElection()
    {
        if (State != AgendaItemState.UnderDiscussion)
            throw new InvalidOperationException("Cannot start an election.");

        if (Election != null)
            throw new InvalidOperationException("Election already in progress.");

        // Pass current candidates to the election session
        Election = new Election()
        {
            OrganizationId = OrganizationId,
            TenantId = TenantId
        };

        Election.StartElection();

        VotingStartedAt = DateTimeOffset.UtcNow;
        State = AgendaItemState.Voting;
    }

    [Throws(typeof(InvalidOperationException))]
    public void EndElection()
    {
        if (Election == null)
            throw new InvalidOperationException("No active election session.");

        Election.TallyBallots();

        Election.EndElection();

        VotingEndedAt = DateTimeOffset.UtcNow;
        IsVoteCompleted = true;
        State = AgendaItemState.Completed;
    }

    public Election? GetCurrentElection()
    {
        return Election?.EndTime == null ? Election : null;
    }

    [Throws(typeof(InvalidOperationException))]
    public void Complete()
    {
        if (!CanComplete)
        {
            throw new InvalidOperationException("Agenda item cannot be completed.");
        }

        State = AgendaItemState.Completed;
    }

    [Throws(typeof(InvalidOperationException))]
    public void Skip()
    {
        if (IsMandatory)
        {
            throw new InvalidOperationException("Mandatory agenda items cannot be postponed.");
        }

        State = AgendaItemState.Skipped;
    }

    [Throws(typeof(InvalidOperationException))]
    public void Postpone()
    {
        if (IsMandatory)
        {
            throw new InvalidOperationException("Mandatory agenda items cannot be postponed.");
        }

        State = AgendaItemState.Postponed;
    }

    [Throws(typeof(InvalidOperationException))]
    public void Cancel()
    {
        if (IsMandatory)
        {
            throw new InvalidOperationException("Mandatory agenda items cannot be canceled.");
        }

        State = AgendaItemState.Canceled;
    }

    private void ValidateAgendaItemType()
    {
        /*
        switch (Type)
        {
            case AgendaItemType.Motion:
                if (VoteActions == VoteActions.None)
                    throw new InvalidOperationException("Motion items must have voting actions.");
                break;
                
            case AgendaItemType.Election:
                if (_candidates.Count == 0)
                    throw new InvalidOperationException("Election items must have candidates.");
                break;
            // Add validations for other types as needed.
            default:
                break;
        }
        */
    }

    // Determines if the item can start discussion
    public bool CanStartDiscussion =>
        (State == AgendaItemState.Pending || State == AgendaItemState.Postponed) &&
        DiscussionActions != DiscussionActions.None &&
        !IsDiscussionCompleted;

    // Determines if the item can start voting
    public bool CanStartVoting =>
        State == AgendaItemState.UnderDiscussion &&
        VoteActions != VoteActions.None &&
        (DiscussionActions != DiscussionActions.Required || IsDiscussionCompleted) &&
        !IsVoteCompleted;

    // Determines if the item can be marked as completed
    public bool CanComplete =>
        (DiscussionActions != DiscussionActions.Required || IsDiscussionCompleted) &&
        (VoteActions != VoteActions.Required || IsVoteCompleted) &&
        (State != AgendaItemState.Completed && State != AgendaItemState.Canceled && State != AgendaItemState.Skipped);

    public IReadOnlyCollection<AgendaItem> SubItems => _subItems;

    [Throws(typeof(InvalidOperationException))]
    public AgendaItem AddItem(AgendaItemType type, string title, string description, Election? election = null)
    {
        if (_subItems.Any(i => i.Title.Equals(title, StringComparison.OrdinalIgnoreCase)))
        {
            throw new InvalidOperationException("An agenda item with the same title already exists.");
        }

        /*
        if (State != AgendaState.InDraft && State != AgendaState.Published)
        {
            throw new InvalidOperationException("Cannot add agenda items unless the agenda is in draft or under review.");
        } */

        int order = 1;

        try
        {
            var last = _subItems.OrderByDescending(x => x.Order).First();
            order = last.Order + 1;
        }
        catch { }

        var item = new AgendaItem(OrganizationId, type, title, description);
        item.AgendaId = AgendaId;
        item.Order = order;
        item.Election = election;

        _subItems.Add(item);
        return item;
    }

    public bool RemoveAgendaItem(AgendaItem item)
    {
        int i = 1;
        var r = _subItems.Remove(item);
        foreach (var item0 in _subItems)
        {
            item0.Order = i++;
        }
        return r;
    }

    [Throws(typeof(InvalidOperationException))]
    [Throws(typeof(ArgumentOutOfRangeException))]
    public bool ReorderAgendaItem(AgendaItem agendaItem, int newOrderPosition)
    {
        if (!_subItems.Contains(agendaItem))
        {
            throw new InvalidOperationException("Agenda item does not exist in this agenda.");
        }

        if (newOrderPosition < 1 || newOrderPosition > _subItems.Count)
        {
            throw new ArgumentOutOfRangeException(nameof(newOrderPosition), "New order position is out of range.");
        }

        int oldOrderPosition = agendaItem.Order;

        if (oldOrderPosition == newOrderPosition)
            return false;

        // Flyttar objektet uppåt i listan
        if (newOrderPosition < oldOrderPosition)
        {
            var itemsToIncrement = SubItems
                .Where(i => i.Order >= newOrderPosition && i.Order < oldOrderPosition)
                .ToList();

            foreach (var item in itemsToIncrement)
            {
                item.Order += 1;
            }
        }
        // Flyttar objektet nedåt i listan
        else
        {
            var itemsToDecrement = SubItems
                .Where(i => i.Order > oldOrderPosition && i.Order <= newOrderPosition)
                .ToList();

            foreach (var item in itemsToDecrement)
            {
                item.Order -= 1;
            }
        }

        // Uppdatera order för objektet som flyttas
        agendaItem.Order = newOrderPosition;

        return true;
    }

    public void Reset()
    {
        State = AgendaItemState.Pending;
        IsDiscussionCompleted = false;
        IsVoteCompleted = false;
        DiscussionStartedAt = null;
        DiscussionEndedAt = null;
        VotingStartedAt = null;
        VotingEndedAt = null;

        Discussion?.Reset();
        Voting = null;
        Election = null;
    }

    public User? CreatedBy { get; set; } = null!;
    public UserId? CreatedById { get; set; } = null!;
    public DateTimeOffset Created { get; set; }
    public User? LastModifiedBy { get; set; }
    public UserId? LastModifiedById { get; set; }
    public DateTimeOffset? LastModified { get; set; }
}