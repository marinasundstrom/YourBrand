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

    public AgendaItemId? AgendaItemId { get; set; }

    public IReadOnlyCollection<SpeakerRequest> SpeakerQueue => _speakerQueue;
    public TimeSpan? SpeakingTimeLimit { get; set; }

    public SpeakerRequest AddSpeaker(MeetingParticipant participant)
    {
        var request = new SpeakerRequest
        {
            OrganizationId = OrganizationId,
            ParticipantId = participant.Id,
            RequestedTime = DateTimeOffset.UtcNow
        };

        _speakerQueue.Add(request);

        return request;
    }

    public SpeakerRequestId RemoveSpeaker(MeetingParticipant participant)
    {
        var request = _speakerQueue.First(s => s.ParticipantId == participant.Id);
        _speakerQueue.Remove(request);
        return request.Id;
    }

    public User? CreatedBy { get; set; } = null!;
    public UserId? CreatedById { get; set; } = null!;
    public DateTimeOffset Created { get; set; }
    public User? LastModifiedBy { get; set; }
    public UserId? LastModifiedById { get; set; }
    public DateTimeOffset? LastModified { get; set; }
}
