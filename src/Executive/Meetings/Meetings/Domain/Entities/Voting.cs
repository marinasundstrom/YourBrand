using YourBrand.Auditability;
using YourBrand.Domain;
using YourBrand.Identity;
using YourBrand.Meetings.Domain.ValueObjects;
using YourBrand.Tenancy;

namespace YourBrand.Meetings.Domain.Entities;

public enum VotingType
{
    SimpleMajority,
    SuperMajority
}

public enum VotingState
{
    NotStarted,
    Voting,
    Completed,
    ResultReady,
    RedoRequired
}

public sealed class Voting : AggregateRoot<VotingId>, IAuditableEntity<VotingId>, IHasTenant, IHasOrganization
{
    private readonly HashSet<Vote> _votes = new HashSet<Vote>();

    public Voting(VotingType type) : base(new VotingId())
    {
        Type = type;
        State = VotingState.NotStarted;
    }

    public TenantId TenantId { get; set; }
    public OrganizationId OrganizationId { get; set; }
    public AgendaItemId? AgendaItemId { get; set; }
    public MotionOperativeClauseId? MotionOperativeClauseId { get; set; }
    public DateTimeOffset? StartTime { get; private set; }
    public DateTimeOffset? EndTime { get; private set; }

    public VotingType Type { get; set; }
    public VotingState State { get; private set; }

    public IReadOnlyCollection<Vote> Votes => _votes;

    public bool HasPassed { get; private set; }

    public void StartVoting()
    {
        if (State != VotingState.NotStarted && State != VotingState.RedoRequired)
            throw new InvalidOperationException("The voting session cannot be started in the current state.");

        StartTime = DateTimeOffset.UtcNow;
        State = VotingState.Voting;
    }

    public void EndVoting()
    {
        if (State != VotingState.Voting)
            throw new InvalidOperationException("Voting must be in progress to end it.");

        EndTime = DateTimeOffset.UtcNow;
        State = VotingState.Completed;
    }

    public void CastVote(MeetingAttendee voter, VoteOption option, TimeProvider timeProvider)
    {
        if (State != VotingState.Voting)
            throw new InvalidOperationException("The voting session is not currently open for voting.");

        if (_votes.Any(v => v.VoterId == voter.Id))
            throw new InvalidOperationException("Attendee has already cast a vote.");

        var vote = new Vote
        {
            OrganizationId = OrganizationId,
            VoterId = voter.Id,
            Option = option,
            TimeCast = timeProvider.GetUtcNow()
        };

        _votes.Add(vote);
    }

    public void TallyVotes()
    {
        if (State != VotingState.Completed)
            throw new InvalidOperationException("Votes can only be tallied after the voting session has ended.");

        var voteCounts = new Dictionary<VoteOption, int>
        {
            { VoteOption.For, 0 },
            { VoteOption.Against, 0 },
            { VoteOption.Abstain, 0 }
        };

        foreach (var vote in Votes)
        {
            if (vote.Option.HasValue)
            {
                voteCounts[vote.Option.Value]++;
            }
        }

        int totalVotes = voteCounts[VoteOption.For] + voteCounts[VoteOption.Against];
        bool isTie = voteCounts[VoteOption.For] == voteCounts[VoteOption.Against];

        if (isTie)
        {
            State = VotingState.RedoRequired;
        }
        else
        {
            if (Type == VotingType.SimpleMajority)
            {
                HasPassed = voteCounts[VoteOption.For] > voteCounts[VoteOption.Against];
            }
            else if (Type == VotingType.SuperMajority)
            {
                HasPassed = voteCounts[VoteOption.For] >= (int)(0.75 * totalVotes);
            }

            State = VotingState.ResultReady;
        }
    }

    public void RedoVoting()
    {
        if (State != VotingState.RedoRequired)
            throw new InvalidOperationException("Redo is only allowed if a tie has occurred.");

        _votes.Clear();
        StartTime = null;
        EndTime = null;
        HasPassed = false;
        State = VotingState.NotStarted;
    }

    public Dictionary<string, int> GetVoteResults()
    {
        if (State != VotingState.ResultReady)
            throw new InvalidOperationException("Results are not ready yet.");

        return _votes.GroupBy(v => v.Option)
                     .ToDictionary(g => g.Key.ToString(), g => g.Count());
    }

    public void Reset()
    {
        State = VotingState.NotStarted;
    }

    public User? CreatedBy { get; set; }
    public UserId? CreatedById { get; set; }
    public DateTimeOffset Created { get; set; }
    public User? LastModifiedBy { get; set; }
    public UserId? LastModifiedById { get; set; }
    public DateTimeOffset? LastModified { get; set; }
}
