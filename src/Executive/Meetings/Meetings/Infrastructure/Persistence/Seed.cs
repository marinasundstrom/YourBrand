using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

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

        await context.Database.EnsureDeletedAsync();
        await context.Database.EnsureCreatedAsync();

        //return;

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

        if (!await context.MeetingGroups.AnyAsync())
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
                role: AttendeeRole.Chairperson,
                userId: TenantConstants.UserAliceId,
                hasSpeakingRights: true,
                hasVotingRights: true
            );

            // Add Bob Smith
            meetingGroup.AddMember(
                name: "Bob Smith",
                email: "bob.smith@example.com",
                role: AttendeeRole.Attendee,
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
                    role: AttendeeRole.Attendee,
                    userId: null,
                    hasSpeakingRights: true,
                    hasVotingRights: true
                );
            }

            context.MeetingGroups.Add(meetingGroup);

            await context.SaveChangesAsync();
        }

        if (!await context.Meetings.AnyAsync())
        {
            var meeting = new Meeting(1, "Board meeting")
            {
                TenantId = TenantConstants.TenantId,
                OrganizationId = TenantConstants.OrganizationId,
                ScheduledAt = DateTimeOffset.UtcNow,
                Location = "HQ",
                Description = "Tes",
                State = MeetingState.Scheduled
            };

            context.Meetings.Add(meeting);

            await context.SaveChangesAsync();
        }

        if (!await context.Agendas.AnyAsync())
        {
            var agenda = new Agenda(1)
            {
                TenantId = TenantConstants.TenantId,
                OrganizationId = TenantConstants.OrganizationId,
                MeetingId = 1,
                //State = AgendaState.InDraft
            };

            // Add agenda items
            agenda.AddItem(
                type: AgendaItemType.CallToOrder,
                title: "Call to Order",
                description: "Chairperson calls the meeting to order."
            );

            agenda.AddItem(
                type: AgendaItemType.RollCall,
                title: "Roll Call",
                description: "Secretary takes attendance."
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

            agenda.AddItem(
                type: AgendaItemType.Reports,
                title: "Committee Reports",
                description: "Presentations from various committees."
            );

            agenda.AddItem(
                type: AgendaItemType.OldBusiness,
                title: "Old Business",
                description: "Discuss unresolved issues from previous meetings."
            );

            agenda.AddItem(
                type: AgendaItemType.NewBusiness,
                title: "New Business",
                description: "Introduce and discuss new topics."
            );

            var item = agenda.AddItem(
                type: AgendaItemType.Motion,
                title: "Motion to Approve Budget",
                description: "Proposal to approve the annual budget."
            );
            item.MotionId = 4;

            var item2 = agenda.AddItem(
                type: AgendaItemType.Motion,
                title: "Motion to Endorse Environmental Initiative",
                description: "Proposal for organization's commitment to environmental sustainability."
            );
            item2.MotionId = 3;

            agenda.AddItem(
                type: AgendaItemType.Adjournment,
                title: "Adjournment",
                description: "Formal closing of the meeting."
            );

            context.Agendas.Add(agenda);

            await context.SaveChangesAsync();
        }
    }
}