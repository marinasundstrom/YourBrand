namespace YourBrand.Meetings;

public enum AgendaItemTypeEnum
{
    CallToOrder = 1,     // Opening the meeting formally
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
