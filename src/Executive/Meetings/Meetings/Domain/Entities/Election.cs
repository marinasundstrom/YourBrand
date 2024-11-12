using YourBrand.Auditability;
using YourBrand.Domain;
using YourBrand.Identity;
using YourBrand.Meetings.Domain.ValueObjects;
using YourBrand.Meetings.Dtos;
using YourBrand.Tenancy;

namespace YourBrand.Meetings.Domain.Entities;

public enum ElectionState
{
    NotStarted,
    Voting,
    Completed,
    ResultReady,
    RedoRequired
}

public sealed class Election : AggregateRoot<ElectionId>, IAuditableEntity<ElectionId>, IHasTenant, IHasOrganization
{
    private readonly HashSet<ElectionCandidate> _candidates = new HashSet<ElectionCandidate>();
    private readonly HashSet<Ballot> _ballots = new HashSet<Ballot>();

    public Election() : base(new ElectionId())
    {

    }

    public TenantId TenantId { get; set; }
    public OrganizationId OrganizationId { get; set; }
    public MeetingId? MeetingId { get; set; }
    public AgendaId? AgendaId { get; set; }
    public AgendaItemId? AgendaItemId { get; set; }
    public MotionOperativeClauseId? MotionOperativeClauseId { get; set; }
    public DateTimeOffset? StartTime { get; private set; }
    public DateTimeOffset? EndTime { get; private set; }

    public MemberRole? Position { get; set; }
    public int? PositionId { get; set; }
    public MeetingGroupId? GroupId { get; set; }

    public int MinimumVotesToWin { get; set; } = 1;
    public ElectionState State { get; private set; } = ElectionState.NotStarted;

    public IReadOnlyCollection<ElectionCandidate> Candidates => _candidates;

    public bool IsAlreadyCandidate(MeetingAttendeeId candidateAttendeeId)
    {
        return Candidates.Any(x => x.AttendeeId == candidateAttendeeId);
    }

    public ElectionCandidate NominateCandidate(
        TimeProvider timeProvider,
        MeetingAttendee attendee, string? statement = null,
        IdGroup? nominatedBy = null)
    {
        return NominateCandidate(timeProvider, attendee.Name!, statement, attendee.UserId, null, attendee.Id, nominatedBy);
    }

    public ElectionCandidate NominateCandidate(
        TimeProvider timeProvider,
        string name, string? statement = null,
        UserId? userId = null, 
        MeetingGroupMemberId? groupMemberId = null,
        MeetingAttendeeId? attendeeId = null, 
        IdGroup? nominatedBy = null)
    {
        if (State != ElectionState.NotStarted)
            throw new InvalidOperationException("New candidates can only be proposed when election not started.");

        var newCandidate = new ElectionCandidate(name, statement, 
            userId, groupMemberId, attendeeId)
        {
            NominatedBy = nominatedBy,
            NominatedAt = timeProvider.GetUtcNow(),
            ElectionId = Id
        };

        _candidates.Add(newCandidate);

        return newCandidate;
    }

    public void WithdrawCandidacy(ElectionCandidate candidate, TimeProvider timeProvider)
    {
        if (State != ElectionState.Voting)
            throw new InvalidOperationException("Candidates can only withdraw during an active voting session.");

        if (!_candidates.Contains(candidate))
            throw new InvalidOperationException("Candidate is not part of this election.");

        candidate.WithdrawnAt = timeProvider.GetUtcNow();
    }

    public IReadOnlyCollection<Ballot> Ballots => _ballots;

    public ElectionCandidate? ElectedCandidate { get; private set; }
    public ElectionCandidateId? ElectedCandidateId { get; private set; }

    public void StartElection()
    {
        if (State != ElectionState.NotStarted && State != ElectionState.RedoRequired)
            throw new InvalidOperationException("The election cannot be started in the current state.");

        StartTime = DateTimeOffset.UtcNow;
        State = ElectionState.Voting;
    }

    public void EndElection()
    {
        if (State != ElectionState.Voting)
            throw new InvalidOperationException("The election must be in progress to end.");

        EndTime = DateTimeOffset.UtcNow;
        State = ElectionState.Completed;
    }

    public void TallyBallots()
    {
        if (State != ElectionState.Completed)
            throw new InvalidOperationException("Cannot tally votes before the election has ended.");

        var voteCounts = new Dictionary<ElectionCandidateId, int>();

        foreach (var ballot in Ballots)
        {
            if (ballot.SelectedCandidateId.HasValue)
            {
                var candidateId = ballot.SelectedCandidateId.Value;
                voteCounts[candidateId] = voteCounts.GetValueOrDefault(candidateId) + 1;
            }
        }

        var eligibleCandidates = voteCounts.Where(vc => vc.Value >= MinimumVotesToWin).ToList();

        if (eligibleCandidates.Any())
        {
            var maxVotes = eligibleCandidates.Max(vc => vc.Value);
            var winners = eligibleCandidates.Where(vc => vc.Value == maxVotes).ToList();

            if (winners.Count == 1)
            {
                ElectedCandidate = _candidates.FirstOrDefault(c => c.Id == winners.First().Key);
                State = ElectionState.ResultReady;
            }
            else
            {
                ElectedCandidate = null; // Tie
                State = ElectionState.RedoRequired;
            }
        }
        else
        {
            State = ElectionState.ResultReady;
        }
    }

    public void RedoElection()
    {
        if (State != ElectionState.RedoRequired)
            throw new InvalidOperationException("Redo is only allowed if a tie has occurred.");

        _ballots.Clear();
        StartTime = null;
        EndTime = null;
        ElectedCandidate = null;
        State = ElectionState.NotStarted;
    }

    public void CastBallot(MeetingAttendee voter, ElectionCandidate candidate, TimeProvider timeProvider)
    {
        if (State != ElectionState.Voting)
            throw new InvalidOperationException("The election is not currently open for voting.");

        if (_ballots.Any(b => b.VoterId == voter.Id))
            throw new InvalidOperationException("Attendee has already cast a ballot.");

        var ballot = new Ballot
        {
            OrganizationId = OrganizationId,
            VoterId = voter.Id,
            SelectedCandidateId = candidate.Id,
            TimeCast = timeProvider.GetUtcNow()
        };

        _ballots.Add(ballot);
    }

    public Dictionary<string, int> GetElectionResults()
    {
        if (State != ElectionState.ResultReady)
            throw new InvalidOperationException("The results are not ready.");

        return _candidates
            .Where(c => c.WithdrawnAt == null) // Exclude withdrawn candidates from results
            .ToDictionary(
                candidate => candidate.Name,
                candidate => _ballots.Count(b => b.SelectedCandidateId == candidate.Id));
    }

    public void Reset()
    {
        State = ElectionState.NotStarted;
    }

    public User? CreatedBy { get; set; }
    public UserId? CreatedById { get; set; }
    public DateTimeOffset Created { get; set; }
    public User? LastModifiedBy { get; set; }
    public UserId? LastModifiedById { get; set; }
    public DateTimeOffset? LastModified { get; set; }
}
