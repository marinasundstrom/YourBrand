using YourBrand.Identity;
using YourBrand.Tenancy;
using YourBrand.Domain;
using YourBrand.Meetings.Domain.ValueObjects;

namespace YourBrand.Meetings.Domain.Entities;

public sealed class SpeakerRequest : Entity<SpeakerRequestId>, IAuditable, IHasTenant, IHasOrganization
{
    public SpeakerRequest()
        : base(new SpeakerRequestId())
    {

    }

    public TenantId TenantId { get; set; }
    public OrganizationId OrganizationId { get; set; }
    public SpeakerSessionId SpeakerSessionId { get; set; }

    public MeetingParticipantId ParticipantId { get; set; }
    public DateTimeOffset RequestedTime { get; set; }
    public TimeSpan? ActualSpeakingTime { get; set; }

    public User? CreatedBy { get; set; } = null!;
    public UserId? CreatedById { get; set; } = null!;
    public DateTimeOffset Created { get; set; }
    public User? LastModifiedBy { get; set; }
    public UserId? LastModifiedById { get; set; }
    public DateTimeOffset? LastModified { get; set; }
}