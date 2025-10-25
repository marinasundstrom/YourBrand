using Shouldly;
using System;
using System.Linq;
using YourBrand.Meetings.Domain.Entities;
using Xunit;

namespace YourBrand.Meetings.Domain.Tests;

public sealed class VotingProcedureTests
{
    [Fact]
    public void StartVoting_WhenAgendaItemActive_InitializesVotingSession()
    {
        // Arrange
        var meeting = MeetingTestFactory.CreateMeetingWithAgenda(AgendaItemType.Motion);

        meeting.StartMeeting();

        var agendaItem = meeting.GetCurrentAgendaItem();
        agendaItem.ShouldNotBeNull();
        agendaItem!.Activate();

        // Act
        agendaItem.StartVoting();

        // Assert
        agendaItem.Phase.ShouldBe(AgendaItemPhase.Voting);
        agendaItem.Voting.ShouldNotBeNull();
        agendaItem.Voting!.State.ShouldBe(VotingState.Voting);
        agendaItem.Voting.StartTime.ShouldNotBeNull();
        agendaItem.IsVoteCompleted.ShouldBeFalse();
    }

    [Fact]
    public void StartVoting_WhenSessionAlreadyInProgress_Throws()
    {
        // Arrange
        var meeting = MeetingTestFactory.CreateMeetingWithAgenda(AgendaItemType.Motion);

        meeting.StartMeeting();

        var agendaItem = meeting.GetCurrentAgendaItem();
        agendaItem.ShouldNotBeNull();
        agendaItem!.Activate();
        agendaItem.StartVoting();

        // Act & Assert
        Should.Throw<InvalidOperationException>(() => agendaItem.StartVoting());
    }

    [Fact]
    public void EndVoting_WithSimpleMajority_CompletesAgendaItemAndSetsResult()
    {
        // Arrange
        var meeting = MeetingTestFactory.CreateMeetingWithAgenda(AgendaItemType.Motion);
        meeting.AddAttendee("Member 1", "member-1", "member1@example.com", AttendeeRole.Member, hasSpeakingRights: true, hasVotingRights: true);
        meeting.AddAttendee("Member 2", "member-2", "member2@example.com", AttendeeRole.Member, hasSpeakingRights: true, hasVotingRights: true);

        meeting.StartMeeting();

        var agendaItem = meeting.GetCurrentAgendaItem();
        agendaItem.ShouldNotBeNull();
        agendaItem!.Activate();
        agendaItem.StartVoting();

        var voters = meeting.Attendees.ToList();
        agendaItem.Voting!.CastVote(voters[0], VoteOption.For, TimeProvider.System);
        agendaItem.Voting.CastVote(voters[1], VoteOption.For, TimeProvider.System);
        agendaItem.Voting.CastVote(voters[2], VoteOption.Against, TimeProvider.System);

        // Act
        agendaItem.EndVoting();

        // Assert
        agendaItem.State.ShouldBe(AgendaItemState.Completed);
        agendaItem.Phase.ShouldBe(AgendaItemPhase.Ended);
        agendaItem.IsVoteCompleted.ShouldBeTrue();
        agendaItem.Voting!.State.ShouldBe(VotingState.ResultReady);
        agendaItem.Voting.HasPassed.ShouldBeTrue();
    }

    [Fact]
    public void EndVoting_WithTie_SetsRedoRequiredAndKeepsAgendaItemActive()
    {
        // Arrange
        var meeting = MeetingTestFactory.CreateMeetingWithAgenda(AgendaItemType.Motion);
        meeting.AddAttendee("Member 1", "member-1", "member1@example.com", AttendeeRole.Member, hasSpeakingRights: true, hasVotingRights: true);

        meeting.StartMeeting();

        var agendaItem = meeting.GetCurrentAgendaItem();
        agendaItem.ShouldNotBeNull();
        agendaItem!.Activate();
        agendaItem.StartVoting();

        var voters = meeting.Attendees.ToList();
        agendaItem.Voting!.CastVote(voters[0], VoteOption.For, TimeProvider.System);
        agendaItem.Voting.CastVote(voters[1], VoteOption.Against, TimeProvider.System);

        // Act
        agendaItem.EndVoting();

        // Assert
        agendaItem.State.ShouldBe(AgendaItemState.Active);
        agendaItem.Phase.ShouldBe(AgendaItemPhase.Voting);
        agendaItem.IsVoteCompleted.ShouldBeFalse();
        agendaItem.Voting!.State.ShouldBe(VotingState.RedoRequired);
    }

    [Fact]
    public void RedoVoting_AfterTie_ResetsVotingSession()
    {
        // Arrange
        var meeting = MeetingTestFactory.CreateMeetingWithAgenda(AgendaItemType.Motion);
        meeting.AddAttendee("Member 1", "member-1", "member1@example.com", AttendeeRole.Member, hasSpeakingRights: true, hasVotingRights: true);

        meeting.StartMeeting();

        var agendaItem = meeting.GetCurrentAgendaItem();
        agendaItem.ShouldNotBeNull();
        agendaItem!.Activate();
        agendaItem.StartVoting();

        var voters = meeting.Attendees.ToList();
        agendaItem.Voting!.CastVote(voters[0], VoteOption.For, TimeProvider.System);
        agendaItem.Voting.CastVote(voters[1], VoteOption.Against, TimeProvider.System);

        agendaItem.EndVoting();
        agendaItem.Voting!.State.ShouldBe(VotingState.RedoRequired);

        // Act
        agendaItem.Voting.RedoVoting();

        // Assert
        agendaItem.Voting!.State.ShouldBe(VotingState.NotStarted);
        agendaItem.Voting.Votes.ShouldBeEmpty();
        agendaItem.Voting.StartTime.ShouldBeNull();
        agendaItem.Voting.EndTime.ShouldBeNull();
        agendaItem.Voting.HasPassed.ShouldBeFalse();
    }
}
