using YourBrand.Identity;
using YourBrand.Tenancy;
using YourBrand.Domain;
using YourBrand.Meetings.Domain.ValueObjects;

namespace YourBrand.Meetings.Domain.Entities;

public sealed class SpeakerSession : AggregateRoot<SpeakerSessionId>, IAuditable, IHasTenant, IHasOrganization
{
    readonly HashSet<SpeakerRequest> _speakerQueue = new HashSet<SpeakerRequest>();

    public SpeakerSession()
        : base(new SpeakerSessionId())
    {

    }

    public TenantId TenantId { get; set; }
    public OrganizationId OrganizationId { get; set; }

    public IReadOnlyCollection<SpeakerRequest> SpeakerQueue => _speakerQueue;
    public TimeSpan? SpeakingTimeLimit { get; set; }

    public void AddSpeaker(MeetingParticipant participant)
    {
        _speakerQueue.Add(new SpeakerRequest
        {
            ParticipantId = participant.Id,
            RequestedTime = DateTimeOffset.UtcNow
        });
    }

    public void RemoveSpeaker(MeetingParticipant participant)
    {
        _speakerQueue.RemoveWhere(s => s.ParticipantId == participant.Id);
    }

    public User? CreatedBy { get; set; } = null!;
    public UserId? CreatedById { get; set; } = null!;
    public DateTimeOffset Created { get; set; }
    public User? LastModifiedBy { get; set; }
    public UserId? LastModifiedById { get; set; }
    public DateTimeOffset? LastModified { get; set; }
}
