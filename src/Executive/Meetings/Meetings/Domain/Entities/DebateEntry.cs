using YourBrand.Identity;
using YourBrand.Tenancy;
using YourBrand.Domain;
using YourBrand.Meetings.Domain.ValueObjects;

namespace YourBrand.Meetings.Domain.Entities;

public class DebateEntry : AggregateRoot<DebateEntryId>, IAuditable, IHasTenant, IHasOrganization
{
    readonly HashSet<DebateEntry> _replies = new HashSet<DebateEntry>();

    public TenantId TenantId { get; set; }
    public OrganizationId OrganizationId { get; set; }

    public DebateId DebateId { get; set; }
    public MeetingParticipantId ParticipantId { get; set; }
    public string Content { get; set; }
    public DateTimeOffset Timestamp { get; set; }
    public DebateEntryId ReplyToEntryId { get; set; }

    public IReadOnlyCollection<DebateEntry> Replies => _replies;

    public User? CreatedBy { get; set; } = null!;
    public UserId? CreatedById { get; set; } = null!;
    public DateTimeOffset Created { get; set; }
    public User? LastModifiedBy { get; set; }
    public UserId? LastModifiedById { get; set; }
    public DateTimeOffset? LastModified { get; set; }
}