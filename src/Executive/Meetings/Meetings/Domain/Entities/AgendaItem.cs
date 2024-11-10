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
    readonly HashSet<ElectionCandidate> _candidates = new HashSet<ElectionCandidate>();
    private HashSet<AgendaItem> _subItems = new HashSet<AgendaItem>();

    protected AgendaItem() {}

    public AgendaItem(AgendaItemType type, string title, string description)
    : base(new AgendaItemId())
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Title is required.", nameof(title));

        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("Description is required.", nameof(description));

        Type = type;
        Title = title;
        Description = description;
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

    public SpeakerSession? SpeakerSession { get; set; }
    public Voting? VotingSession { get; set; }
    public Election? ElectionSession { get; set; }

    public DateTimeOffset? DiscussionStartedAt { get; private set; }
    public DateTimeOffset? DiscussionEndedAt { get; private set; }
    public DateTimeOffset? VotingStartedAt { get; private set; }
    public DateTimeOffset? VotingEndedAt { get; private set; }

    // For motions
    public MotionId? MotionId { get; set; }

    // For election
    public string? Position { get; set; }
    
    public IReadOnlyCollection<ElectionCandidate> Candidates => _candidates;

    public void AddCandidate(MeetingAttendee candidate, string? statement)
    {
        if (candidate == null)
            throw new ArgumentNullException(nameof(candidate));

        if (State != AgendaItemState.Pending)
            throw new InvalidOperationException("Candidates can only be managed when the agenda item is pending.");

        if (_candidates.Any(c => c.AttendeeId == candidate.Id))
            throw new InvalidOperationException("Attendee is already a candidate.");

        _candidates.Add(new ElectionCandidate(candidate.Id, candidate.Name, statement));
    }

    public void RemoveCandidate(ElectionCandidate candidate)
    {
        if (State != AgendaItemState.Pending)
            throw new InvalidOperationException("Candidates can only be managed when the agenda item is pending.");

        if (!_candidates.Contains(candidate))
            throw new InvalidOperationException("Is not a candidate.");

        _candidates.Remove(candidate);
    }

    public void StartDiscussion()
    {
        if (State != AgendaItemState.Pending && State != AgendaItemState.Postponed)
            throw new InvalidOperationException("Discussion can only be started when the agenda item is pending or postponed.");

        if (IsDiscussionCompleted)
            throw new InvalidOperationException("Discussion already completed.");

        SpeakerSession = new SpeakerSession { OrganizationId = OrganizationId };
        SpeakerSession.StartSession();

        DiscussionStartedAt = DateTimeOffset.UtcNow;
        State = AgendaItemState.UnderDiscussion;
    }

    public void EndDiscussion()
    {
        SpeakerSession?.EndSession();

        IsDiscussionCompleted = true;
        DiscussionEndedAt = DateTimeOffset.UtcNow;
    }

    public void StartVoting()
    {
        if (State != AgendaItemState.UnderDiscussion)
            throw new InvalidOperationException("Cannot start voting.");

        if (VotingSession != null)
            throw new InvalidOperationException("Voting session already in progress.");

        VotingSession = new Voting(VotingType.SimpleMajority)
        {
            OrganizationId = OrganizationId,
            TenantId = TenantId
        };

        VotingSession.StartVoting();

        VotingStartedAt = DateTimeOffset.UtcNow;
        State = AgendaItemState.Voting;
    }

    public void EndVoting()
    {
        if (VotingSession == null)
            throw new InvalidOperationException("No active voting session.");

        VotingSession.TallyVotes();

        VotingSession.EndVoting();

        VotingEndedAt = DateTimeOffset.UtcNow;
        IsVoteCompleted = true;
        State = AgendaItemState.Completed;
    }

    public Voting? GetCurrentVotingSession()
    {
        return VotingSession?.EndTime == null ? VotingSession : null;
    }

    public void StartElection()
    {
        if (State != AgendaItemState.UnderDiscussion)
            throw new InvalidOperationException("Cannot start an election.");

        if (ElectionSession != null)
            throw new InvalidOperationException("Election already in progress.");

        // Pass current candidates to the election session
        ElectionSession = new Election(_candidates)
        {
            OrganizationId = OrganizationId,
            TenantId = TenantId
        };

        ElectionSession.StartElection();

        VotingStartedAt = DateTimeOffset.UtcNow;
        State = AgendaItemState.Voting;
    }

    public void EndElection()
    {
        if (ElectionSession == null)
            throw new InvalidOperationException("No active election session.");

        ElectionSession.TallyBallots();

        ElectionSession.EndElection();

        VotingEndedAt = DateTimeOffset.UtcNow;
        IsVoteCompleted = true;
        State = AgendaItemState.Completed;
    }

    public Election? GetCurrentElection()
    {
        return ElectionSession?.EndTime == null ? ElectionSession : null;
    }

    public void Complete()
    {
        if (!CanComplete)
        {
            throw new InvalidOperationException("Agenda item cannot be completed.");
        }

        State = AgendaItemState.Completed;
    }

    public void Skip()
    {
        if (IsMandatory)
        {
            throw new InvalidOperationException("Mandatory agenda items cannot be postponed.");
        }

        State = AgendaItemState.Skipped;
    }

    public void Postpone()
    {
        if (IsMandatory)
        {
            throw new InvalidOperationException("Mandatory agenda items cannot be postponed.");
        }

        State = AgendaItemState.Postponed;
    }

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

    public AgendaItem AddItem(AgendaItemType type, string title, string description)
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

        var item = new AgendaItem(type, title, description);
        item.AgendaId = AgendaId;
        item.Order = order;
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

        SpeakerSession?.Reset();
        VotingSession = null;
        ElectionSession = null;
    }

    public User? CreatedBy { get; set; } = null!;
    public UserId? CreatedById { get; set; } = null!;
    public DateTimeOffset Created { get; set; }
    public User? LastModifiedBy { get; set; }
    public UserId? LastModifiedById { get; set; }
    public DateTimeOffset? LastModified { get; set; }
}