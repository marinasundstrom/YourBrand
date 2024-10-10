using YourBrand.Identity;
using YourBrand.Tenancy;
using YourBrand.Domain;
using YourBrand.Meetings.Domain.ValueObjects;

namespace YourBrand.Meetings.Domain.Entities;

public sealed class VotingSession : AggregateRoot<VotingSessionId>, IAuditable, IHasTenant, IHasOrganization
{
    readonly HashSet<Vote> _votes = new HashSet<Vote>();

    public VotingSession()
        : base(new VotingSessionId())
    {

    }

    public TenantId TenantId { get; set; }
    public OrganizationId OrganizationId { get; set; }

    public DateTimeOffset? StartTime { get; set; }
    public DateTimeOffset? EndTime { get; set; }

    public IReadOnlyCollection<Vote> Votes => _votes;

    public void AddVote(Vote vote)
    {
        if (_votes.Any(v => v.VoterId == vote.VoterId))
            throw new InvalidOperationException("Participant has already voted.");

        _votes.Add(vote);
    }

    public void TallyVotes(AgendaItemType type, out Dictionary<string, int> results)
    {
        results = new Dictionary<string, int>();

        if (type == AgendaItemType.Motion)
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
        else if (type == AgendaItemType.Election)
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

