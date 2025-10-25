using Shouldly;
using System;
using System.Linq;
using YourBrand.Domain;
using YourBrand.Meetings.Domain.Entities;
using YourBrand.Meetings.Domain.ValueObjects;
using YourBrand.Tenancy;
using Xunit;

namespace YourBrand.Meetings.Domain.Tests;

public sealed class MeetingProcedureTests
{
    [Fact]
    public void MeetingProcedure_FullLifecycle_TransitionsStatesAndPhases()
    {
        // Arrange
        var meeting = CreateMeetingWithAgenda(
            AgendaItemType.PublicComment,
            AgendaItemType.Motion);

        var agendaItems = meeting.Agenda!.Items.OrderBy(i => i.Order).ToList();
        var discussionItem = agendaItems[0];
        var motionItem = agendaItems[1];

        discussionItem.DiscussionActions = DiscussionActions.Required;
        discussionItem.VoteActions = VoteActions.Optional;

        motionItem.DiscussionActions = DiscussionActions.Required;
        motionItem.VoteActions = VoteActions.Required;

        // Act
        meeting.StartMeeting();

        var currentItem = meeting.GetCurrentAgendaItem();
        currentItem.ShouldBe(discussionItem);
        currentItem!.State.ShouldBe(AgendaItemState.Pending);
        currentItem.Phase.ShouldBe(AgendaItemPhase.Default);

        currentItem.Activate();
        currentItem.State.ShouldBe(AgendaItemState.Active);

        currentItem.StartDiscussion();
        currentItem.Phase.ShouldBe(AgendaItemPhase.Discussion);

        currentItem.EndDiscussion();
        currentItem.Phase.ShouldBe(AgendaItemPhase.Default);
        currentItem.IsDiscussionCompleted.ShouldBeTrue();

        currentItem.CanComplete.ShouldBeTrue();
        currentItem.Complete();
        currentItem.State.ShouldBe(AgendaItemState.Completed);
        currentItem.Phase.ShouldBe(AgendaItemPhase.Ended);

        var nextItem = meeting.MoveToNextAgendaItem();
        nextItem.ShouldBe(motionItem);
        nextItem.State.ShouldBe(AgendaItemState.Active);
        nextItem.Phase.ShouldBe(AgendaItemPhase.Default);

        nextItem.StartDiscussion();
        nextItem.Phase.ShouldBe(AgendaItemPhase.Discussion);

        nextItem.EndDiscussion();
        nextItem.IsDiscussionCompleted.ShouldBeTrue();
        nextItem.Phase.ShouldBe(AgendaItemPhase.Default);

        nextItem.StartVoting();
        nextItem.Phase.ShouldBe(AgendaItemPhase.Voting);
        nextItem.Voting.ShouldNotBeNull();

        var voter = meeting.Attendees.First();
        nextItem.Voting!.CastVote(voter, VoteOption.For, TimeProvider.System);

        nextItem.EndVoting();
        nextItem.State.ShouldBe(AgendaItemState.Completed);
        nextItem.Phase.ShouldBe(AgendaItemPhase.Ended);
        nextItem.IsVoteCompleted.ShouldBeTrue();

        meeting.CurrentAgendaItemIndex.ShouldBe(1);

        Should.Throw<InvalidOperationException>(() => meeting.MoveToNextAgendaItem());
    }

    [Fact]
    public void MoveToNextAgendaItem_ReactivatesPostponedItems()
    {
        // Arrange
        var meeting = CreateMeetingWithAgenda(
            AgendaItemType.ChairpersonRemarks,
            AgendaItemType.Reports,
            AgendaItemType.Announcements);

        var agendaItems = meeting.Agenda!.Items.OrderBy(i => i.Order).ToList();
        var first = agendaItems[0];
        var second = agendaItems[1];
        var third = agendaItems[2];

        second.Postpone();
        second.State.ShouldBe(AgendaItemState.Postponed);

        meeting.StartMeeting();

        var currentItem = meeting.GetCurrentAgendaItem();
        currentItem.ShouldBe(first);

        currentItem!.Activate();
        currentItem.Complete();
        currentItem.State.ShouldBe(AgendaItemState.Completed);

        // Act
        var activatedSecond = meeting.MoveToNextAgendaItem();

        // Assert
        activatedSecond.ShouldBe(second);
        activatedSecond.State.ShouldBe(AgendaItemState.Active);
        activatedSecond.Phase.ShouldBe(AgendaItemPhase.Default);

        activatedSecond.Complete();
        activatedSecond.State.ShouldBe(AgendaItemState.Completed);

        var activatedThird = meeting.MoveToNextAgendaItem();
        activatedThird.ShouldBe(third);
        activatedThird.State.ShouldBe(AgendaItemState.Active);
        activatedThird.Phase.ShouldBe(AgendaItemPhase.Default);
    }

    private static Meeting CreateMeetingWithAgenda(params AgendaItemType[] itemTypes)
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
