using YourBrand.Identity;
using YourBrand.Tenancy;
using YourBrand.Domain;
using YourBrand.Meetings.Domain.ValueObjects;

namespace YourBrand.Meetings.Domain.Entities;

public enum AgendaItemType
{
    CallToOrder,         // Opening the meeting formally
    RollCall,            // Attendance check for quorum
    ApprovalOfMinutes,   // Approval of previous meeting's minutes
    ApprovalOfAgenda,    // Approval of the current meeting's agenda
    ConsentAgenda,       // Routine items grouped for a single vote
    ChairpersonRemarks,  // Opening remarks or statements by the chairperson
    PublicComment,       // Time allocated for public comments (if applicable)
    Reports,             // Presentation of reports (e.g., financial, committee reports)
    FinancialReport,     // Specific item for financial reports
    SpecialPresentations,// Formal presentations by guests or special contributors
    OldBusiness,         // Items carried over from previous meetings
    UnfinishedBusiness,  // Ongoing items from previous meetings
    NewBusiness,         // Introduction of new topics for discussion or decision
    Announcements,       // Formal announcements to the assembly
    Motion,              // Proposals requiring a decision or vote
    Discussion,          // Structured discussion on specific agenda topics
    StrategicDiscussion, // Forward-looking strategy sessions or brainstorming
    Voting,              // Formal voting on motions or decisions
    Election,            // Formal election
    ExecutiveSession,    // Private session for confidential matters
    RecognitionAwards,   // Formal recognition or award announcements
    ActionItemsReview,   // Review of action items from previous meetings
    Recess,              // Planned break or recess during the meeting
    GuestSpeakers,       // Slot for invited guest speakers
    FollowUpItems,       // Review of follow-up actions and decisions
    Adjournment,         // Formal closing of the meeting
    ClosingRemarks       // Concluding remarks by key figures
}

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

public class AgendaItem : Entity<AgendaItemId>, IAuditable, IHasTenant, IHasOrganization
{
    readonly HashSet<ElectionCandidate> _candidates = new HashSet<ElectionCandidate>();

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

    public AgendaItemType Type { get; set; }

    public string Title { get; set; }
    public string Description { get; set; }
    public AgendaItemState State { get; set; } = AgendaItemState.Pending;
    public int Order  { get; set; }

    public bool IsMandatory { get; set; }
    public DiscussionActions DiscussionActions { get; set; } = DiscussionActions.Optional;
    public VoteActions VoteActions { get; set; } = VoteActions.Optional;

    public bool IsDiscussionCompleted { get; private set; }
    public bool IsVoteCompleted { get; private set; }

    // public AgendaItemId DependsOnItem { get; set; }

    public SpeakerSession? SpeakerSession { get; set; }
    public VotingSession? VotingSession { get; set; }

    public DateTimeOffset? DiscussionStartedAt { get; private set; }
    public DateTimeOffset? DiscussionEndedAt { get; private set; }
    public DateTimeOffset? VotingStartedAt { get; private set; }
    public DateTimeOffset? VotingEndedAt { get; private set; }

    // For motions
    public MotionId? MotionId { get; set; }

    // For election
    public string? Position { get; set; }
    public IReadOnlyCollection<ElectionCandidate> Candidates => _candidates;

    public void AddCandidate(MeetingAttendee candidate, string statement)
    {
        if (candidate == null)
        {
            throw new ArgumentNullException(nameof(candidate));
        }

        if (string.IsNullOrWhiteSpace(statement))
        {
            throw new ArgumentException("Candidate statement is required.", nameof(statement));
        }

        if (Type != AgendaItemType.Election)
        {
            throw new InvalidOperationException("Candidates can only be added to election agenda items.");
        }

        if (_candidates.Any(v => v.NomineeId == candidate.Id))
        {
            throw new InvalidOperationException("Attendee is already a candidate.");
        }

        _candidates.Add(new ElectionCandidate(candidate.Id, statement));
    }

    public void StartDiscussion()
    {
        if (State != AgendaItemState.Pending)
        {
            throw new InvalidOperationException("Cannot start discussion.");
        }

        if (State != AgendaItemState.Pending && State != AgendaItemState.Postponed)
        {
            throw new InvalidOperationException("Discussion can only be started when the agenda item is pending or postponed.");
        }

        if (State == AgendaItemState.Completed || State == AgendaItemState.Canceled || State == AgendaItemState.Skipped)
        {
            throw new InvalidOperationException("Cannot perform this action on an agenda item that is completed, canceled, or skipped.");
        }

        if (State == AgendaItemState.Postponed || State == AgendaItemState.Canceled)
        {
            throw new InvalidOperationException("Cannot perform this action on an agenda item that is postponed or canceled.");
        }

        if (IsDiscussionCompleted)
        {
            throw new InvalidOperationException("Already had voting.");
        }

        SpeakerSession = new SpeakerSession();
        SpeakerSession.OrganizationId = OrganizationId;

        DiscussionStartedAt = DateTimeOffset.UtcNow;

        State = AgendaItemState.UnderDiscussion;
    }

    public void EndDiscussion()
    {
        IsDiscussionCompleted = true;
        DiscussionEndedAt = DateTimeOffset.UtcNow;
    }

    public void StartVoting()
    {
        if (State != AgendaItemState.UnderDiscussion)
        {
            throw new InvalidOperationException("Cannot start voting.");
        }

        if (DiscussionActions == DiscussionActions.Required && !IsDiscussionCompleted)
        {
            throw new InvalidOperationException("Discussion must be completed before voting can start.");
        }

        if (State == AgendaItemState.Completed || State == AgendaItemState.Canceled || State == AgendaItemState.Skipped)
        {
            throw new InvalidOperationException("Cannot perform this action on an agenda item that is completed, canceled, or skipped.");
        }

        if (State == AgendaItemState.Postponed || State == AgendaItemState.Canceled)
        {
            throw new InvalidOperationException("Cannot perform this action on an agenda item that is postponed or canceled.");
        }

        if (IsVoteCompleted) 
        {
            throw new InvalidOperationException("Already had voting.");
        }

        VotingSession = new VotingSession((Type) switch 
        {   
            AgendaItemType.Motion => VotingType.Motion,
            AgendaItemType.Election => VotingType.Election,
            _ => VotingType.Motion //throw new InvalidOperationException("Invalid agenda item type")
        });
        VotingSession.OrganizationId = OrganizationId;

        VotingStartedAt = DateTimeOffset.UtcNow;

        State = AgendaItemState.Voting;
    }

    public void EndVoting()
    {
        IsVoteCompleted = true;
        VotingEndedAt = DateTimeOffset.UtcNow;
    }

    public void Complete()
    {
        if (DiscussionActions == DiscussionActions.Required && !IsDiscussionCompleted)
        {
            throw new InvalidOperationException("Discussion is required and has not been completed.");
        }

        if (VoteActions == VoteActions.Required && !IsVoteCompleted)
        {
            throw new InvalidOperationException("Voting is required and has not been completed.");
        }

        State = AgendaItemState.Completed;
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
        if (Type == AgendaItemType.Motion && VoteActions == VoteActions.None)
        {
            throw new InvalidOperationException("Motion items must have voting actions.");
        }
        // Add similar validations for other types as needed
    }

    public User? CreatedBy { get; set; } = null!;
    public UserId? CreatedById { get; set; } = null!;
    public DateTimeOffset Created { get; set; }
    public User? LastModifiedBy { get; set; }
    public UserId? LastModifiedById { get; set; }
    public DateTimeOffset? LastModified { get; set; }
}