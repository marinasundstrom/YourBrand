using YourBrand.Domain;
using YourBrand.Meetings.Domain.Entities;
using YourBrand.Meetings.Domain.ValueObjects;
using YourBrand.Tenancy;

namespace YourBrand.Meetings.Domain.Tests;

internal static class MeetingTestFactory
{
    public static Meeting CreateMeetingWithAgenda(params AgendaItemType[] itemTypes)
    {
        var tenantId = new TenantId("tenant");
        var organizationId = new OrganizationId("org");
        var meetingId = new MeetingId(1);

        var meeting = new Meeting(meetingId, "Test meeting")
        {
            TenantId = tenantId,
            OrganizationId = organizationId,
            State = MeetingState.Scheduled
        };

        meeting.Agenda = CreateAgenda(itemTypes, tenantId, organizationId, meeting.Id);

        meeting.AddAttendee("Chair", "chair-user", "chair@example.com", AttendeeRole.Member, hasSpeakingRights: true, hasVotingRights: true);

        return meeting;
    }

    private static Agenda CreateAgenda(AgendaItemType[] itemTypes, TenantId tenantId, OrganizationId organizationId, MeetingId meetingId)
    {
        var agenda = new Agenda(new AgendaId(1))
        {
            TenantId = tenantId,
            OrganizationId = organizationId,
            MeetingId = meetingId
        };

        for (int index = 0; index < itemTypes.Length; index++)
        {
            var type = itemTypes[index];
            var item = agenda.AddItem(type, $"Item {index + 1}", $"Description {index + 1}");

            item.IsMandatory = type.IsMandatory;
            item.DiscussionActions = type.RequiresDiscussion ? DiscussionActions.Required : DiscussionActions.Optional;
            item.VoteActions = type.RequiresVoting ? VoteActions.Required : VoteActions.Optional;
        }

        return agenda;
    }
}
