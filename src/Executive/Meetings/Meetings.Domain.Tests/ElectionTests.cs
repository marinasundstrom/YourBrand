using Shouldly;
using System;
using System.Linq;
using YourBrand.Domain;
using YourBrand.Meetings.Domain.Entities;
using YourBrand.Meetings.Domain.ValueObjects;
using YourBrand.Tenancy;
using Xunit;

namespace YourBrand.Meetings.Domain.Tests;

public sealed class ElectionTests
{
    [Fact]
    public void StartElection_SetsStateToVotingAndStartTime()
    {
        var election = CreateElection();
        var candidate = election.NominateCandidate(TimeProvider.System, "Alice");

        election.StartElection();

        election.State.ShouldBe(ElectionState.Voting);
        election.StartTime.ShouldNotBeNull();
        election.Candidates.ShouldContain(candidate);
    }

    [Fact]
    public void StartElection_WhenAlreadyVoting_Throws()
    {
        var election = CreateElection();
        election.NominateCandidate(TimeProvider.System, "Alice");
        election.StartElection();

        Should.Throw<InvalidOperationException>(() => election.StartElection());
    }

    [Fact]
    public void CastBallot_WhenVoting_AddsBallot()
    {
        var election = CreateElection();
        var candidate = election.NominateCandidate(TimeProvider.System, "Alice");
        var voter = CreateAttendee("Voter 1");

        election.StartElection();
        election.CastBallot(voter, candidate, TimeProvider.System);

        election.Ballots.Count.ShouldBe(1);
        election.Ballots.Single().VoterId.ShouldBe(voter.Id);
    }

    [Fact]
    public void CastBallot_WhenVoterAlreadyCast_Throws()
    {
        var election = CreateElection();
        var candidate = election.NominateCandidate(TimeProvider.System, "Alice");
        var voter = CreateAttendee("Voter 1");

        election.StartElection();
        election.CastBallot(voter, candidate, TimeProvider.System);

        Should.Throw<InvalidOperationException>(() => election.CastBallot(voter, candidate, TimeProvider.System));
    }

    [Fact]
    public void TallyBallots_WithClearWinner_SetsElectedCandidate()
    {
        var election = CreateElection();
        var alice = election.NominateCandidate(TimeProvider.System, "Alice");
        var bob = election.NominateCandidate(TimeProvider.System, "Bob");

        election.StartElection();
        election.CastBallot(CreateAttendee("Voter 1"), alice, TimeProvider.System);
        election.CastBallot(CreateAttendee("Voter 2"), alice, TimeProvider.System);
        election.CastBallot(CreateAttendee("Voter 3"), bob, TimeProvider.System);

        election.EndElection();
        election.TallyBallots();

        election.State.ShouldBe(ElectionState.ResultReady);
        election.ElectedCandidate.ShouldBe(alice);
    }

    [Fact]
    public void TallyBallots_WithTie_SetsRedoRequired()
    {
        var election = CreateElection();
        var alice = election.NominateCandidate(TimeProvider.System, "Alice");
        var bob = election.NominateCandidate(TimeProvider.System, "Bob");

        election.StartElection();
        election.CastBallot(CreateAttendee("Voter 1"), alice, TimeProvider.System);
        election.CastBallot(CreateAttendee("Voter 2"), bob, TimeProvider.System);

        election.EndElection();
        election.TallyBallots();

        election.State.ShouldBe(ElectionState.RedoRequired);
        election.ElectedCandidate.ShouldBeNull();
    }

    [Fact]
    public void RedoElection_AfterTie_ResetsElection()
    {
        var election = CreateElection();
        var alice = election.NominateCandidate(TimeProvider.System, "Alice");
        var bob = election.NominateCandidate(TimeProvider.System, "Bob");

        election.StartElection();
        election.CastBallot(CreateAttendee("Voter 1"), alice, TimeProvider.System);
        election.CastBallot(CreateAttendee("Voter 2"), bob, TimeProvider.System);

        election.EndElection();
        election.TallyBallots();
        election.State.ShouldBe(ElectionState.RedoRequired);

        election.RedoElection();

        election.State.ShouldBe(ElectionState.NotStarted);
        election.Ballots.ShouldBeEmpty();
        election.StartTime.ShouldBeNull();
        election.EndTime.ShouldBeNull();
        election.ElectedCandidate.ShouldBeNull();
    }

    [Fact]
    public void WithdrawCandidacy_DuringVoting_SetsWithdrawnAt()
    {
        var election = CreateElection();
        var alice = election.NominateCandidate(TimeProvider.System, "Alice");

        election.StartElection();
        election.WithdrawCandidacy(alice, TimeProvider.System);

        alice.WithdrawnAt.ShouldNotBeNull();
    }

    [Fact]
    public void GetElectionResults_ExcludesWithdrawnCandidates()
    {
        var election = CreateElection();
        var alice = election.NominateCandidate(TimeProvider.System, "Alice");
        var bob = election.NominateCandidate(TimeProvider.System, "Bob");

        election.StartElection();
        election.CastBallot(CreateAttendee("Voter 1"), alice, TimeProvider.System);
        election.CastBallot(CreateAttendee("Voter 2"), alice, TimeProvider.System);
        election.CastBallot(CreateAttendee("Voter 3"), bob, TimeProvider.System);

        election.WithdrawCandidacy(bob, TimeProvider.System);

        election.EndElection();
        election.TallyBallots();

        var results = election.GetElectionResults();

        results.Count.ShouldBe(1);
        results.ShouldContainKeyAndValue("Alice", 2);
        results.ContainsKey("Bob").ShouldBeFalse();
    }

    private static Election CreateElection()
    {
        return new Election
        {
            TenantId = new TenantId("tenant"),
            OrganizationId = new OrganizationId("org"),
            MinimumVotesToWin = 1
        };
    }

    private static MeetingAttendee CreateAttendee(string name)
    {
        return new MeetingAttendee
        {
            TenantId = new TenantId("tenant"),
            OrganizationId = new OrganizationId("org"),
            MeetingId = new MeetingId(1),
            Name = name,
            HasVotingRights = true,
            IsPresent = true,
            Role = AttendeeRole.Member,
            RoleId = AttendeeRole.Member.Id
        };
    }
}
