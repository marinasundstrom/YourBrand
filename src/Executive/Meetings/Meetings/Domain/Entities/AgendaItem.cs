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
    ChairpersonRemarks,  // Opening remarks or statements by the chairperson
    PublicComment,       // Time allocated for public comments (if applicable)
    Reports,             // Presentation of reports (e.g., financial, committee reports)
    SpecialPresentations,// Formal presentations by guests or special contributors
    OldBusiness,         // Items carried over from previous meetings
    NewBusiness,         // Introduction of new topics for discussion or decision
    Announcements,       // Formal announcements to the assembly
    Motion,              // Proposals requiring a decision or vote
    Discussion,          // Structured discussion on specific agenda topics
    Voting,              // Formal voting on motions or decisions
    Election,            // Formal election
    ExecutiveSession,    // Private session for confidential matters
    Adjournment          // Formal closing of the meeting
}

public enum AgendaItemState
{
    Pending,
    UnderDiscussion,
    Voting,
    Completed,
    Postponed,
    Cancelled
}

public class AgendaItem : Entity<AgendaItemId>, IAuditable, IHasTenant, IHasOrganization
{
    readonly HashSet<ElectionCandidate> _candidates = new HashSet<ElectionCandidate>();

    public AgendaItem(AgendaItemType type, string title, string description)
    : base(new AgendaItemId())
    {
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
    public int Order { get; set; }

    //public SpeakerSession? SpeakerSession { get; set; }
    public VotingSession? VotingSession { get; set; }
    public VotingSessionId? VotingSessionId { get; set; }

    // For motions
    public MotionId? MotionId { get; set; }

    // For election
    public string? Position { get; set; }
    public IReadOnlyCollection<ElectionCandidate> Candidates => _candidates;

    public void AddCandidate(MeetingParticipant candidate, string statement)
    {
        if (_candidates.Any(v => v.NomineeId == candidate.Id))
            throw new InvalidOperationException("Participant is already a candidate.");

        _candidates.Add(new ElectionCandidate(candidate.Id, statement));
    }

    public void StartDiscussion()
    {
        if (State != AgendaItemState.Pending)
        {
            throw new InvalidOperationException("Cannot start discussion.");
        }

        State = AgendaItemState.UnderDiscussion;
    }

    public void StartVoting()
    {
        if (State != AgendaItemState.UnderDiscussion)
        {
            throw new InvalidOperationException("Cannot start voting.");
        }

        VotingSession = new VotingSession();

        State = AgendaItemState.Voting;
    }

    public void CompleteAgendaItem()
    {
        if (State != AgendaItemState.Voting)
        {
            throw new InvalidOperationException("Agenda item voting not completed.");
        }

        State = AgendaItemState.Completed;
    }

    public User? CreatedBy { get; set; } = null!;
    public UserId? CreatedById { get; set; } = null!;
    public DateTimeOffset Created { get; set; }
    public User? LastModifiedBy { get; set; }
    public UserId? LastModifiedById { get; set; }
    public DateTimeOffset? LastModified { get; set; }
}