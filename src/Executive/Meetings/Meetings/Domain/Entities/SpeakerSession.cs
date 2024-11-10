using YourBrand.Auditability;
using YourBrand.Domain;
using YourBrand.Identity;
using YourBrand.Meetings.Domain.ValueObjects;
using YourBrand.Tenancy;

namespace YourBrand.Meetings.Domain.Entities;

public sealed class SpeakerSession : AggregateRoot<SpeakerSessionId>, IAuditableEntity<SpeakerSessionId>, IHasTenant, IHasOrganization
{
    readonly List<SpeakerRequest> _speakerQueue = new List<SpeakerRequest>();

    public SpeakerSession()
        : base(new SpeakerSessionId())
    {

    }

    public TenantId TenantId { get; set; }
    public OrganizationId OrganizationId { get; set; }

    public AgendaItemId? AgendaItemId { get; set; }

    public IReadOnlyCollection<SpeakerRequest> SpeakerQueue => _speakerQueue;
    public TimeSpan? SpeakingTimeLimit { get; set; }


    // Method to get the current speaker (first in the queue)
    public SpeakerRequest? GetCurrentSpeaker()
    {
        return _speakerQueue.FirstOrDefault();
    }

    public SpeakerRequest AddSpeaker(MeetingAttendee attendee)
    {
        if (_speakerQueue.Any(s => s.AttendeeId == attendee.Id))
        {
            throw new InvalidOperationException("Attendee already has a pending speaker request.");
        }

        var request = new SpeakerRequest
        {
            OrganizationId = OrganizationId,
            AttendeeId = attendee.Id,
            Name = attendee.Name!,
            RequestedTime = DateTimeOffset.UtcNow
        };

        _speakerQueue.Add(request);

        return request;
    }

    public SpeakerRequestId RemoveSpeaker(MeetingAttendee attendee)
    {
        var request = _speakerQueue.FirstOrDefault(s => s.AttendeeId == attendee.Id);
        if (request == null)
        {
            throw new InvalidOperationException("Speaker request not found for the specified attendee.");
        }

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