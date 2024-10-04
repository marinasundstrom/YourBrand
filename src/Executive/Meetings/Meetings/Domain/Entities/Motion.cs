using YourBrand.Identity;
using YourBrand.Tenancy;
using YourBrand.Domain;
using YourBrand.Meetings.Domain.ValueObjects;

namespace YourBrand.Meetings.Domain.Entities;

public enum MotionStatus { Proposed }

public class Motion : AggregateRoot<MotionId>, IAuditable, IHasTenant, IHasOrganization
{
    readonly HashSet<Vote> _votes = new HashSet<Vote>();

    public TenantId TenantId { get; set; }
    public OrganizationId OrganizationId { get; set; }

    public string Text { get; set; }
    public MotionStatus Status { get; set; } = MotionStatus.Proposed;
    public AgendaItemId AgendaItemId { get; set; }
    public IReadOnlyCollection<Vote> Votes => _votes;

    public DebateId DebateId { get; set; }

    public void AddVote(Vote vote)
    {
        if (Votes.Any(v => v.VoterId == vote.VoterId))
            throw new InvalidOperationException("Participant has already voted.");
        _votes.Add(vote);
    }

    public bool IsVotingOpen { get; set; } = false;

    public void OpenVoting()
    {
        IsVotingOpen = true;
    }

    public void CloseVoting()
    {
        IsVotingOpen = false;
        // Calculate results
    }

    public User? CreatedBy { get; set; } = null!;
    public UserId? CreatedById { get; set; } = null!;
    public DateTimeOffset Created { get; set; }
    public User? LastModifiedBy { get; set; }
    public UserId? LastModifiedById { get; set; }
    public DateTimeOffset? LastModified { get; set; }
}