namespace YourBrand.Meetings.Domain.Entities;

public class AgendaItemType
{
    public int Id { get; private set; } // Unique identifier
    public string Name { get; private set; } // Name of the agenda item type
    public string? Description { get; private set; } // Optional description

    // Rule properties
    public bool RequiresDiscussion { get; private set; }
    public bool RequiresVoting { get; private set; }
    public bool CanBePostponed { get; private set; }
    public bool CanBeSkipped { get; private set; }
    public bool IsMandatory { get; private set; }

    protected AgendaItemType() { }

    private AgendaItemType(int id, string name, string? description = null,
        bool requiresDiscussion = false, bool requiresVoting = false,
        bool canBePostponed = true, bool canBeSkipped = true, bool isMandatory = false)
    {
        Id = id;
        Name = name;
        Description = description;
        RequiresDiscussion = requiresDiscussion;
        RequiresVoting = requiresVoting;
        CanBePostponed = canBePostponed;
        CanBeSkipped = canBeSkipped;
        IsMandatory = isMandatory;
    }

    // Static instances with rule settings
    public static readonly AgendaItemType CallToOrder = new(1, "Call To Order", "Opening the meeting formally", isMandatory: true, canBePostponed: false, canBeSkipped: false);
    public static readonly AgendaItemType RollCall = new(2, "Roll Call", "Attendance check", isMandatory: true, canBePostponed: false, canBeSkipped: false);
    public static readonly AgendaItemType QuorumCheck = new(3, "Quorum Check", "Verification of quorum", isMandatory: true, canBePostponed: false, canBeSkipped: false);
    public static readonly AgendaItemType ApprovalOfMinutes = new(4, "Approval Of Minutes", "Approval of previous meeting's minutes", requiresVoting: true);
    public static readonly AgendaItemType ApprovalOfAgenda = new(5, "Approval Of Agenda", "Approval of the current meeting's agenda", requiresVoting: true, canBePostponed: false);
    public static readonly AgendaItemType ConsentAgenda = new(6, "Consent Agenda", "Routine items grouped for a single vote", requiresVoting: true);
    public static readonly AgendaItemType ChairpersonRemarks = new(7, "Chairperson Remarks", "Opening remarks by the chairperson");
    public static readonly AgendaItemType PublicComment = new(8, "Public Comment", "Time allocated for public comments", requiresDiscussion: true);
    public static readonly AgendaItemType Reports = new(9, "Reports", "Presentation of reports");
    public static readonly AgendaItemType FinancialReport = new(10, "Financial Report", "Specific item for financial reports");
    public static readonly AgendaItemType SpecialPresentations = new(11, "Special Presentations", "Formal presentations", requiresDiscussion: true);
    public static readonly AgendaItemType OldBusiness = new(12, "Old Business", "Items from previous meetings", requiresDiscussion: true);
    public static readonly AgendaItemType UnfinishedBusiness = new(13, "Unfinished Business", "Ongoing items from previous meetings", requiresDiscussion: true);
    public static readonly AgendaItemType NewBusiness = new(14, "New Business", "Introduction of new topics", requiresDiscussion: true);
    public static readonly AgendaItemType Announcements = new(15, "Announcements", "Formal announcements");
    public static readonly AgendaItemType Motion = new(16, "Motion", "Proposals requiring a vote", requiresVoting: true, isMandatory: true);
    public static readonly AgendaItemType Discussion = new(17, "Discussion", "Structured discussion", requiresDiscussion: true);
    public static readonly AgendaItemType StrategicDiscussion = new(18, "Strategic Discussion", "Strategy sessions", requiresDiscussion: true);
    public static readonly AgendaItemType Voting = new(19, "Voting", "Formal voting on motions", requiresVoting: true);
    public static readonly AgendaItemType Election = new(20, "Election", "Formal elections", requiresVoting: true, canBePostponed: false, canBeSkipped: false);
    public static readonly AgendaItemType ExecutiveSession = new(21, "Executive Session", "Private session for confidential matters", requiresDiscussion: true, canBePostponed: false, canBeSkipped: false);
    public static readonly AgendaItemType RecognitionAwards = new(22, "Recognition Awards", "Recognition or awards");
    public static readonly AgendaItemType ActionItemsReview = new(23, "Action Items Review", "Review of action items", requiresDiscussion: true);
    public static readonly AgendaItemType Recess = new(24, "Recess", "Planned break", canBeSkipped: true);  // Allow recess to be skipped
    public static readonly AgendaItemType GuestSpeakers = new(25, "Guest Speakers", "Guest speakers", requiresDiscussion: true);
    public static readonly AgendaItemType FollowUpItems = new(26, "Follow Up Items", "Review of follow-up actions", requiresDiscussion: true);
    public static readonly AgendaItemType Adjournment = new(27, "Adjournment", "Closing of the meeting", isMandatory: true, canBePostponed: false, canBeSkipped: false);
    public static readonly AgendaItemType ClosingRemarks = new(28, "Closing Remarks", "Concluding remarks");

    // List of all types for reference
    public static readonly AgendaItemType[] AllTypes = {
        CallToOrder, RollCall, QuorumCheck, ApprovalOfMinutes, ApprovalOfAgenda, ConsentAgenda,
        ChairpersonRemarks, PublicComment, Reports, FinancialReport, SpecialPresentations,
        OldBusiness, UnfinishedBusiness, NewBusiness, Announcements, Motion, Discussion,
        StrategicDiscussion, Voting, Election, ExecutiveSession, RecognitionAwards,
        ActionItemsReview, Recess, GuestSpeakers, FollowUpItems, Adjournment, ClosingRemarks
    };

    // Methods for rule validation
    public void ValidateStartDiscussion()
    {
        if (!RequiresDiscussion)
        {
            throw new InvalidOperationException($"{Name} does not require a discussion phase.");
        }
    }

    public void ValidateStartVoting()
    {
        if (!RequiresVoting)
        {
            throw new InvalidOperationException($"{Name} does not require a voting phase.");
        }
    }

    public void ValidatePostpone()
    {
        if (!CanBePostponed || IsMandatory)
        {
            throw new InvalidOperationException($"{Name} cannot be postponed.");
        }
    }

    public void ValidateSkip()
    {
        if (!CanBeSkipped || IsMandatory)
        {
            throw new InvalidOperationException($"{Name} cannot be skipped.");
        }
    }

    public static bool operator ==(AgendaItemType? left, AgendaItemType? right) => Equals(left, right);
    public static bool operator !=(AgendaItemType? left, AgendaItemType? right) => !Equals(left, right);

    public override bool Equals(object? obj) => obj is AgendaItemType other && Id == other.Id;
    public override int GetHashCode() => Id.GetHashCode();
}
