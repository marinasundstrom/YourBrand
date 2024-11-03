using YourBrand.Auditability;
using YourBrand.Domain;
using YourBrand.Identity;
using YourBrand.Meetings.Domain.ValueObjects;
using YourBrand.Tenancy;

namespace YourBrand.Meetings.Domain.Entities;

public sealed class SpeakerSession : AggregateRoot<SpeakerSessionId>, IAuditableEntity<SpeakerSessionId>, IHasTenant, IHasOrganization
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

    public SpeakerRequest AddSpeaker(MeetingAttendee attendee)
    {
        var request = new SpeakerRequest
        {
            OrganizationId = OrganizationId,
            AttendeeId = attendee.Id,
            RequestedTime = DateTimeOffset.UtcNow
        };

        _speakerQueue.Add(request);

        return request;
    }

    public SpeakerRequestId RemoveSpeaker(MeetingAttendee attendee)
    {
        var request = _speakerQueue.First(s => s.AttendeeId == attendee.Id);
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