using YourBrand.Domain;
using YourBrand.Identity;
using YourBrand.Meetings.Domain.ValueObjects;
using YourBrand.Tenancy;

namespace YourBrand.Meetings.Domain.Entities;

public enum VotingType
{
    Motion,
    Election
}

public sealed class VotingSession : AggregateRoot<VotingSessionId>, IAuditable, IHasTenant, IHasOrganization
{
    readonly HashSet<Vote> _votes = new HashSet<Vote>();

    public VotingSession(VotingType type)
        : base(new VotingSessionId())
    {
        Type = type;
    }

    public TenantId TenantId { get; set; }
    public OrganizationId OrganizationId { get; set; }


    //public AgendaId? AgendaId { get; set; }
    public AgendaItemId? AgendaItemId { get; set; }
    //public MotionId? MotionId { get; set; }
    public MotionOperativeClauseId? MotionOperativeClauseId { get; set; }

    public VotingType Type { get; set; }

    public DateTimeOffset? StartTime { get; set; }
    public DateTimeOffset? EndTime { get; set; }

    public IReadOnlyCollection<Vote> Votes => _votes;

    public void AddVote(Vote vote)
    {
        if (_votes.Any(v => v.VoterId == vote.VoterId))
            throw new InvalidOperationException("Attendee has already voted.");

        _votes.Add(vote);
    }

    public void TallyVotes(out Dictionary<string, int> results)
    {
        results = new Dictionary<string, int>();

        if (Type == VotingType.Motion)
        {
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

            foreach (var item in voteCounts)
            {
                results[item.Key.ToString()] = item.Value;
            }
        }
        else if (Type == VotingType.Election)
        {
            foreach (var vote in Votes)
            {
                if (vote.SelectedCandidate != null)
                {
                    var candidateId = vote.SelectedCandidate.NomineeId;
                    if (!results.ContainsKey(candidateId))
                    {
                        results[candidateId] = 0;
                    }
                    results[candidateId]++;
                }
            }
        }
    }

    public User? CreatedBy { get; set; } = null!;
    public UserId? CreatedById { get; set; } = null!;
    public DateTimeOffset Created { get; set; }
    public User? LastModifiedBy { get; set; }
    public UserId? LastModifiedById { get; set; }
    public DateTimeOffset? LastModified { get; set; }
}