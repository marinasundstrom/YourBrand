using System;
using System.Linq;
using Shouldly;
using YourBrand.Meetings.Domain.Entities;
using Xunit;

namespace YourBrand.Meetings.Domain.Tests;

public sealed class DiscussionProcedureTests
{
    [Fact]
    public void StartDiscussion_WhenAgendaItemActiveInitializesDiscussion()
    {
        var meeting = MeetingTestFactory.CreateMeetingWithAgenda(AgendaItemType.NewBusiness);

        meeting.StartMeeting();
        var agendaItem = meeting.GetCurrentAgendaItem();

        agendaItem.ShouldNotBeNull();
        agendaItem!.Activate();

        agendaItem.StartDiscussion();

        agendaItem.Phase.ShouldBe(AgendaItemPhase.Discussion);
        agendaItem.Discussion.ShouldNotBeNull();
        agendaItem.Discussion!.State.ShouldBe(DiscussionState.InProgress);
        agendaItem.DiscussionStartedAt.ShouldNotBeNull();
        agendaItem.Discussion!.SpeakerQueue.ShouldBeEmpty();
    }

    [Fact]
    public void RequestSpeakerSlot_WhenPendingInitializesDiscussionAndAddsSpeaker()
    {
        var meeting = MeetingTestFactory.CreateMeetingWithAgenda(AgendaItemType.NewBusiness);
        var agendaItem = meeting.Agenda!.Items.First();
        var attendee = meeting.Attendees.First();

        agendaItem.RequestSpeakerSlot(attendee);

        agendaItem.Discussion.ShouldNotBeNull();
        var speakerRequest = agendaItem.Discussion!.SpeakerQueue.ShouldHaveSingleItem();
        speakerRequest.AttendeeId.ShouldBe(attendee.Id);
        speakerRequest.Status.ShouldBe(SpeakerRequestStatus.Pending);
    }

    [Fact]
    public void EndDiscussion_CompletesDiscussionAndMarksSpeakersCompleted()
    {
        var meeting = MeetingTestFactory.CreateMeetingWithAgenda(AgendaItemType.NewBusiness);

        meeting.StartMeeting();
        var agendaItem = meeting.GetCurrentAgendaItem();
        agendaItem.ShouldNotBeNull();
        var attendee = meeting.Attendees.First();

        agendaItem!.RequestSpeakerSlot(attendee);
        agendaItem.Activate();
        agendaItem.StartDiscussion();
        agendaItem.Discussion!.MoveToNextSpeaker();

        agendaItem.EndDiscussion();

        agendaItem.IsDiscussionCompleted.ShouldBeTrue();
        agendaItem.Phase.ShouldBe(AgendaItemPhase.Default);
        agendaItem.DiscussionEndedAt.ShouldNotBeNull();
        agendaItem.Discussion!.State.ShouldBe(DiscussionState.Completed);

        foreach (var speaker in agendaItem.Discussion!.SpeakerQueue)
        {
            speaker.Status.ShouldBe(SpeakerRequestStatus.Completed);
        }
    }

    [Fact]
    public void StartDiscussion_WhenAgendaItemNotActive_Throws()
    {
        var meeting = MeetingTestFactory.CreateMeetingWithAgenda(AgendaItemType.NewBusiness);

        meeting.StartMeeting();
        var agendaItem = meeting.GetCurrentAgendaItem();
        agendaItem.ShouldNotBeNull();

        Should.Throw<InvalidOperationException>(() => agendaItem!.StartDiscussion());
    }
}
