using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using YourBrand.Meetings.Domain.Entities;
using YourBrand.Tenancy;

namespace YourBrand.Meetings.Infrastructure.Persistence;

public static class Seed
{
    public static async Task SeedData(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();

        var tenantContext = scope.ServiceProvider.GetRequiredService<ISettableTenantContext>();
        tenantContext.SetTenantId(TenantConstants.TenantId);

        using var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var timeProvider = scope.ServiceProvider.GetRequiredService<TimeProvider>();

        await context.Database.EnsureDeletedAsync();
        await context.Database.EnsureCreatedAsync();

        //return;

        if (!await context.AttendeeRoles.AnyAsync())
        {
            await context.AttendeeRoles.AddRangeAsync(AttendeeRole.AllRoles);
            await context.SaveChangesAsync();
        }

        if (!await context.MeetingFunctions.AnyAsync())
        {
            await context.MeetingFunctions.AddRangeAsync(MeetingFunction.AllFunctions);
            await context.SaveChangesAsync();
        }

        if (!await context.AgendaItemTypes.AnyAsync())
        {
            await context.AgendaItemTypes.AddRangeAsync(AgendaItemType.AllTypes);
            await context.SaveChangesAsync();
        }

        if (!await context.MemberRoles.AnyAsync())
        {
            await context.MemberRoles.AddRangeAsync(MemberRole.AllRoles);
            await context.SaveChangesAsync();
        }

        if (!await context.Motions.AnyAsync())
        {
            var motion1 = new Motion(1, "Motion to Approve New Policy")
            {
                Text = "This motion seeks approval for the new organizational policy on remote work.",
                TenantId = TenantConstants.TenantId,
                OrganizationId = TenantConstants.OrganizationId,
                Status = MotionStatus.Proposal
            };
            motion1.AddOperativeClause(OperativeAction.Decides, "Decides to implement the remote work policy effective immediately.");
            motion1.AddOperativeClause(OperativeAction.Instructs, "Instructs the HR department to update the employee handbook accordingly.");
            context.Motions.Add(motion1);

            var motion2 = new Motion(2, "Motion to Request Funding")
            {
                Text = "This motion requests additional funding for the marketing department.",
                TenantId = TenantConstants.TenantId,
                OrganizationId = TenantConstants.OrganizationId,
                Status = MotionStatus.Proposal
            };
            motion2.AddOperativeClause(OperativeAction.Requests, "Requests an additional budget allocation of $50,000 for Q4 marketing initiatives.");
            motion2.AddOperativeClause(OperativeAction.Urges, "Urges the finance committee to expedite the approval process.");

            context.Motions.Add(motion2);

            var motion3 = new Motion(3, "Motion to Endorse Environmental Initiative")
            {
                Text = "This motion endorses the organization's commitment to environmental sustainability.",
                TenantId = TenantConstants.TenantId,
                OrganizationId = TenantConstants.OrganizationId,
                Status = MotionStatus.Proposal
            };
            motion3.AddOperativeClause(OperativeAction.Endorses, "Endorses the adoption of renewable energy sources for all company facilities.");
            motion3.AddOperativeClause(OperativeAction.CallsFor, "Calls for the reduction of carbon emissions by 25% over the next five years.");
            context.Motions.Add(motion3);

            var motion4 = new Motion(4, "Motion to Approve Budget")
            {
                Text = "This motion seeks approval for the annual budget.",
                TenantId = TenantConstants.TenantId,
                OrganizationId = TenantConstants.OrganizationId,
                Status = MotionStatus.Proposal
            };
            motion4.AddOperativeClause(OperativeAction.Decides, "Decides to approve the proposed annual budget effective immediately.");
            motion4.AddOperativeClause(OperativeAction.Instructs, "Instructs the finance department to implement the budget accordingly.");
            context.Motions.Add(motion4);

            await context.SaveChangesAsync();
        }

        MeetingGroup housingCooperative = default!;

        if (!await context.MeetingGroups.AnyAsync())
        {
            Board(context);

            housingCooperative = HousingCooperative(context);

            await context.SaveChangesAsync();
        }

        Meeting annualGeneralMeeting = default!;

        if (!await context.Meetings.AnyAsync())
        {
            var boardMeeting = new Meeting(1, "Board meeting")
            {
                TenantId = TenantConstants.TenantId,
                OrganizationId = TenantConstants.OrganizationId,
                ScheduledAt = DateTimeOffset.UtcNow,
                Location = "HQ",
                Description = "Test",
                State = MeetingState.Scheduled
            };

            context.Meetings.Add(boardMeeting);

            annualGeneralMeeting = new Meeting(2, "Annual general meeting")
            {
                TenantId = TenantConstants.TenantId,
                OrganizationId = TenantConstants.OrganizationId,
                ScheduledAt = DateTimeOffset.UtcNow,
                Location = "Fancy venue",
                Description = "Test",
                State = MeetingState.Scheduled
            };

            await annualGeneralMeeting.AddAttendeesFromGroup(housingCooperative, context);

            var orderedAttendees = annualGeneralMeeting.Attendees
                .OrderBy(x => x.Order)
                .ToList();

            if (orderedAttendees.Count > 0)
            {
                orderedAttendees[0].SetFunctions(new[] { MeetingFunction.Chairperson });
            }

            if (orderedAttendees.Count > 1)
            {
                orderedAttendees[1].SetFunctions(new[] { MeetingFunction.Secretary });
            }

            context.Meetings.Add(annualGeneralMeeting);

            await context.SaveChangesAsync();
        }

        if (!await context.Agendas.AnyAsync())
        {
            await BoardMeeting(context);

            await AnnualGeneralMeeting(context, annualGeneralMeeting, timeProvider);
        }
    }

    private static MeetingGroup NewBoard(ApplicationDbContext context)
    {
        // Create a new MeetingGroup
        var meetingGroup = new MeetingGroup(3, "Board of directors (2025)", "New board")
        {
            TenantId = TenantConstants.TenantId,
            OrganizationId = TenantConstants.OrganizationId,
            Quorum = new Quorum
            {
                RequiredNumber = 3
            }
        };

        return meetingGroup;
    }

    private static MeetingGroup Board(ApplicationDbContext context)
    {
        // Create a new MeetingGroup
        var meetingGroup = new MeetingGroup(1, "Board of directors", "Group for members of board of directors")
        {
            TenantId = TenantConstants.TenantId,
            OrganizationId = TenantConstants.OrganizationId,
            Quorum = new Quorum
            {
                RequiredNumber = 3
            }
        };

        // Add Alice Smith
        meetingGroup.AddMember(
            name: "Alice Smith",
            email: "alice.smith@example.com",
            role: AttendeeRole.Member,
            userId: TenantConstants.UserAliceId,
            hasSpeakingRights: true,
            hasVotingRights: true
        );

        // Add Bob Smith
        meetingGroup.AddMember(
            name: "Bob Smith",
            email: "bob.smith@example.com",
            role: AttendeeRole.Member,
            userId: TenantConstants.UserBobId,
            hasSpeakingRights: true,
            hasVotingRights: true
        );

        // Add 5 additional members
        for (int i = 1; i <= 5; i++)
        {
            meetingGroup.AddMember(
                name: $"Member {i}",
                email: $"member{i}@example.com",
                role: AttendeeRole.Member,
                userId: null,
                hasSpeakingRights: true,
                hasVotingRights: true
            );
        }

        context.MeetingGroups.Add(meetingGroup);

        return meetingGroup;
    }

    private static MeetingGroup HousingCooperative(ApplicationDbContext context)
    {
        // Create a new MeetingGroup
        var meetingGroup = new MeetingGroup(2, "Housing Cooperative Board", "Board responsible for overseeing the management and operations of the housing cooperative, ensuring a safe and well-maintained community.")
        {
            TenantId = TenantConstants.TenantId,
            OrganizationId = TenantConstants.OrganizationId,
            Quorum = new Quorum
            {
                RequiredNumber = 6
            }
        };

        // Add Alice Smith
        meetingGroup.AddMember(
            name: "Alice Smith",
            email: "alice.smith@example.com",
            role: AttendeeRole.Member,
            userId: TenantConstants.UserAliceId,
            hasSpeakingRights: true,
            hasVotingRights: true
        );

        // Add Bob Smith
        meetingGroup.AddMember(
            name: "Bob Smith",
            email: "bob.smith@example.com",
            role: AttendeeRole.Member,
            userId: TenantConstants.UserBobId,
            hasSpeakingRights: true,
            hasVotingRights: true
        );

        // Add 18 additional members to make a total of 20 members
        for (int i = 1; i <= 18; i++)
        {
            meetingGroup.AddMember(
                name: $"Member {i}",
                email: $"member{i}@example.com",
                role: AttendeeRole.Member,
                userId: null,
                hasSpeakingRights: true,
                hasVotingRights: true
            );
        }

        // Add the meeting group to the context
        context.MeetingGroups.Add(meetingGroup);

        return meetingGroup;
    }

    private static async Task BoardMeeting(ApplicationDbContext context)
    {
        var agenda = new Agenda(1)
        {
            TenantId = TenantConstants.TenantId,
            OrganizationId = TenantConstants.OrganizationId,
            MeetingId = 1,
        };

        // Opening and formalities
        agenda.AddItem(
            type: AgendaItemType.CallToOrder,
            title: "Call to Order",
            description: "Chairperson calls the meeting to order."
        );

        agenda.AddItem(
            type: AgendaItemType.Election,
            title: "Election of Secretary",
            description: "Election to appoint the Secretary for the meeting."
        );

        agenda.AddItem(
            type: AgendaItemType.Election,
            title: "Election of Adjuster of Minutes",
            description: "Election to appoint an Adjuster of Minutes for this meeting."
        );

        agenda.AddItem(
            type: AgendaItemType.RollCall,
            title: "Roll Call",
            description: "Secretary takes attendance."
        );

        agenda.AddItem(
            type: AgendaItemType.QuorumCheck,
            title: "Quorum Check",
            description: "Secretary verifies quorum."
        );

        agenda.AddItem(
            type: AgendaItemType.ApprovalOfMinutes,
            title: "Approval of Minutes",
            description: "Review and approve minutes from the previous meeting."
        );

        agenda.AddItem(
            type: AgendaItemType.ApprovalOfAgenda,
            title: "Approval of Agenda",
            description: "Approve the agenda for the current meeting."
        );

        // Chairperson’s opening remarks
        agenda.AddItem(
            type: AgendaItemType.ChairpersonRemarks,
            title: "Chairperson's Remarks",
            description: "The chairperson provides an overview of key focus areas for the meeting."
        );

        // Key reports
        agenda.AddItem(
            type: AgendaItemType.Reports,
            title: "Financial Report",
            description: "Presentation of the financial performance and status."
        );

        agenda.AddItem(
            type: AgendaItemType.Reports,
            title: "Operational Report",
            description: "Overview of operational activities and performance metrics."
        );

        agenda.AddItem(
            type: AgendaItemType.Reports,
            title: "Committee Reports",
            description: "Updates from various committees, including progress on strategic initiatives."
        );

        // Business discussions and decisions
        agenda.AddItem(
            type: AgendaItemType.OldBusiness,
            title: "Old Business",
            description: "Review unresolved issues from previous meetings."
        );

        agenda.AddItem(
            type: AgendaItemType.NewBusiness,
            title: "New Business",
            description: "Introduce new business items requiring board attention or decision."
        );

        agenda.AddItem(
            type: AgendaItemType.Discussion,
            title: "Strategic Planning Discussion",
            description: "Focused discussion on strategic goals and planning for the next quarter."
        );

        var item = agenda.AddItem(
            type: AgendaItemType.Motion,
            title: "Motion to Approve Key Initiatives",
            description: "Proposal for endorsement of strategic initiatives for growth and sustainability."
        );
        item.MotionId = 3;

        agenda.AddItem(
            type: AgendaItemType.Voting,
            title: "Voting",
            description: "Formal voting on motions and decisions presented during the meeting."
        );

        // Final announcements and closing
        agenda.AddItem(
            type: AgendaItemType.Announcements,
            title: "Announcements",
            description: "Final announcements, including reminders of upcoming meetings or events."
        );

        agenda.AddItem(
            type: AgendaItemType.Adjournment,
            title: "Adjournment",
            description: "Formal closing of the meeting."
        );

        agenda.AddItem(
            type: AgendaItemType.ClosingRemarks,
            title: "Closing Remarks",
            description: "Final comments from the chairperson or other board members."
        );

        // Save the agenda to the context
        context.Agendas.Add(agenda);
        await context.SaveChangesAsync();
    }

    private static async Task AnnualGeneralMeeting(ApplicationDbContext context, Meeting meeting, TimeProvider timeProvider)
    {
        var agenda = new Agenda(2)
        {
            TenantId = TenantConstants.TenantId,
            OrganizationId = TenantConstants.OrganizationId,
            MeetingId = 2,
        };

        // Opening of the AGM
        agenda.AddItem(
            type: AgendaItemType.CallToOrder,
            title: "Call to Order",
            description: "Temporary chairperson calls the meeting to order."
        );

        // Election of AGM roles
        agenda.AddItem(
            type: AgendaItemType.Election,
            title: "Election of Chairperson",
            description: "Election to appoint the Chairperson for the meeting."
        );

        agenda.AddItem(
            type: AgendaItemType.Election,
            title: "Election of Secretary",
            description: "Election to appoint the Secretary for the meeting."
        );

        agenda.AddItem(
            type: AgendaItemType.Election,
            title: "Election of Adjuster of Minutes",
            description: "Election to appoint an Adjuster of Minutes for this meeting."
        );

        // Formalities and approvals
        agenda.AddItem(
            type: AgendaItemType.RollCall,
            title: "Roll Call",
            description: "Secretary takes attendance."
        );

        agenda.AddItem(
            type: AgendaItemType.QuorumCheck,
            title: "Quorum Check",
            description: "Secretary verifies quorum."
        );

        agenda.AddItem(
            type: AgendaItemType.ApprovalOfMinutes,
            title: "Approval of Minutes",
            description: "Review and approve minutes from the previous AGM."
        );

        agenda.AddItem(
            type: AgendaItemType.ApprovalOfAgenda,
            title: "Approval of Agenda",
            description: "Approve the agenda for the current meeting."
        );

        // Chairperson’s overview and annual reports
        agenda.AddItem(
            type: AgendaItemType.ChairpersonRemarks,
            title: "Chairperson's Remarks",
            description: "Chairperson provides an overview of the meeting and addresses key objectives."
        );

        agenda.AddItem(
            type: AgendaItemType.Reports,
            title: "Annual Financial Report",
            description: "Presentation of the annual financial report by the Treasurer."
        );

        agenda.AddItem(
            type: AgendaItemType.Reports,
            title: "Committee Reports",
            description: "Reports from various committees on annual activities and accomplishments."
        );

        // Election of board members and other officials
        var boardMemberElection = agenda.AddItem(
            type: AgendaItemType.Election,
            title: "Election of Board Members",
            description: "Election of board members and officials as per organizational bylaws."
        );

        var election = new Election()
        {
            OrganizationId = TenantConstants.OrganizationId,
            MeetingFunction = MeetingFunction.Chairperson,
            MeetingFunctionId = MeetingFunction.Chairperson.Id,
            GroupId = 3
        };

        election.NominateCandidate(timeProvider, meeting.Attendees.First(x => x.Order == 1), null);
        election.NominateCandidate(timeProvider, meeting.Attendees.First(x => x.Order == 10), null);

        boardMemberElection.AddItem(
            type: AgendaItemType.Election,
            title: "Election of Chairperson",
            description: "Election of the Chairperson of the board as per organizational bylaws.",
            election: election
        );

        election = new Election()
        {
            OrganizationId = TenantConstants.OrganizationId,
            MeetingFunction = MeetingFunction.Facilitator,
            MeetingFunctionId = MeetingFunction.Facilitator.Id,
            GroupId = 3
        };

        election.NominateCandidate(timeProvider, meeting.Attendees.First(x => x.Order == 2), null);

        var item = boardMemberElection.AddItem(
            type: AgendaItemType.Election,
            title: "Election of Vice Chairperson",
            description: "Election of the Vice Chairperson of the board as per organizational bylaws.",
            election: election
        );

        election = new Election()
        {
            OrganizationId = TenantConstants.OrganizationId,
            MeetingFunction = MeetingFunction.Secretary,
            MeetingFunctionId = MeetingFunction.Secretary.Id,
            GroupId = 3
        };

        election.NominateCandidate(timeProvider, meeting.Attendees.First(x => x.Order == 3), null);

        item = boardMemberElection.AddItem(
            type: AgendaItemType.Election,
            title: "Election of Secretary",
            description: "Election of the Secretary of the board as per organizational bylaws.",
            election: election
        );

        election = new Election()
        {
            OrganizationId = TenantConstants.OrganizationId,
            MeetingFunction = MeetingFunction.Timekeeper,
            MeetingFunctionId = MeetingFunction.Timekeeper.Id,
            GroupId = 3
        };

        election.NominateCandidate(timeProvider, meeting.Attendees.First(x => x.Order == 4), null);

        item = boardMemberElection.AddItem(
            type: AgendaItemType.Election,
            title: "Election of Treasurer",
            description: "Election of the Treasurer of the board as per organizational bylaws.",
            election: election
        );

        election = new Election()
        {
            OrganizationId = TenantConstants.OrganizationId,
            MeetingFunctionId = null,
            GroupId = 3
        };

        election.NominateCandidate(timeProvider, meeting.Attendees.First(x => x.Order == 5), null);

        item = boardMemberElection.AddItem(
            type: AgendaItemType.Election,
            title: "Election of Member (Position 1)",
            description: "Election of a board member for Position 1 as per organizational bylaws.",
            election: election
        );

        election = new Election()
        {
            OrganizationId = TenantConstants.OrganizationId,
            MeetingFunctionId = null,
            GroupId = 3
        };

        election.NominateCandidate(timeProvider, meeting.Attendees.First(x => x.Order == 6), null);

        item = boardMemberElection.AddItem(
            type: AgendaItemType.Election,
            title: "Election of Member (Position 2)",
            description: "Election of a board member for Position 2 as per organizational bylaws.",
            election: election
        );

        election = new Election()
        {
            OrganizationId = TenantConstants.OrganizationId,
            MeetingFunctionId = null,
            GroupId = 3
        };

        election.NominateCandidate(timeProvider, meeting.Attendees.First(x => x.Order == 7), null);

        item = boardMemberElection.AddItem(
            type: AgendaItemType.Election,
            title: "Election of Member (Position 3)",
            description: "Election of a board member for Position 3 as per organizational bylaws.",
            election: election
        );

        election = new Election()
        {
            OrganizationId = TenantConstants.OrganizationId,
            MeetingFunctionId = null,
            GroupId = 3
        };

        election.NominateCandidate(timeProvider, meeting.Attendees.First(x => x.Order == 8), null);

        item = boardMemberElection.AddItem(
            type: AgendaItemType.Election,
            title: "Election of Alternate (Position 1)",
            description: "Election of an alternate board member for Position 1 as per organizational bylaws.",
            election: election
        );

        election = new Election()
        {
            OrganizationId = TenantConstants.OrganizationId,
            MeetingFunctionId = null,
            GroupId = 3
        };

        election.NominateCandidate(timeProvider, meeting.Attendees.First(x => x.Order == 9), null);

        item = boardMemberElection.AddItem(
            type: AgendaItemType.Election,
            title: "Election of Alternate (Position 2)",
            description: "Election of an alternate board member for Position 2 as per organizational bylaws.",
            election: election
        );

        agenda.AddItem(
            type: AgendaItemType.Election,
            title: "Election of Auditor",
            description: "Election of an auditor as per organizational bylaws."
        );

        // Reviewing and addressing past business and new issues
        agenda.AddItem(
            type: AgendaItemType.OldBusiness,
            title: "Old Business",
            description: "Discussion of unresolved issues from previous meetings.");

        agenda.AddItem(
            type: AgendaItemType.NewBusiness,
            title: "New Business",
            description: "Introduction and discussion of new business for the upcoming year."
        );

        // Strategic discussion and budget approval
        agenda.AddItem(
            type: AgendaItemType.Discussion,
            title: "Strategic Discussion",
            description: "Strategic planning and goal-setting for the upcoming year."
        );

        var item1 = agenda.AddItem(
            type: AgendaItemType.Motion,
            title: "Approval of Annual Budget",
            description: "Proposal to approve the annual budget."
        );
        item1.MotionId = 4;

        // Other significant motions
        var item2 = agenda.AddItem(
            type: AgendaItemType.Motion,
            title: "Approval of Environmental Initiative",
            description: "Proposal for commitment to environmental sustainability."
        );
        item2.MotionId = 3;

        // Voting on motions and decisions
        agenda.AddItem(
            type: AgendaItemType.Voting,
            title: "Voting",
            description: "Voting on motions and proposals presented during the meeting."
        );

        // Final remarks and closing
        agenda.AddItem(
            type: AgendaItemType.Announcements,
            title: "Announcements",
            description: "Final announcements, such as reminders for future meetings or events."
        );

        agenda.AddItem(
            type: AgendaItemType.Adjournment,
            title: "Adjournment",
            description: "Formal closing of the meeting."
        );

        agenda.AddItem(
            type: AgendaItemType.ClosingRemarks,
            title: "Closing Remarks",
            description: "Final comments from the chairperson or other leaders."
        );

        context.Agendas.Add(agenda);
        await context.SaveChangesAsync();
    }
}