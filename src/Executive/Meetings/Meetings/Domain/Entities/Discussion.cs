using YourBrand.Auditability;
using YourBrand.Domain;
using YourBrand.Identity;
using YourBrand.Meetings.Domain.ValueObjects;
using YourBrand.Tenancy;

namespace YourBrand.Meetings.Domain.Entities;

public enum DiscussionState
{
    NotStarted,
    InProgress,
    Paused,
    Completed
}

public sealed class Discussion : AggregateRoot<DiscussionId>, IAuditableEntity<DiscussionId>, IHasTenant, IHasOrganization
{
    private readonly List<SpeakerRequest> _speakerQueue = new List<SpeakerRequest>();

    public Discussion() : base(new DiscussionId())
    {
        State = DiscussionState.NotStarted;
    }

    public TenantId TenantId { get; set; }
    public OrganizationId OrganizationId { get; set; }
    public AgendaItemId? AgendaItemId { get; set; }

    public DiscussionState State { get; private set; }

    public IEnumerable<SpeakerRequest> SpeakerQueue => _speakerQueue;

    public IEnumerable<SpeakerRequest> GetOrderedSpeakerQueue() => _speakerQueue
        .Where(x => x.Status == SpeakerRequestStatus.Pending)
        .OrderBy(x => x.RequestedTime);

    public TimeSpan? SpeakingTimeLimit { get; set; }

    // Property to store the current speaker
    public SpeakerRequest? CurrentSpeaker { get; private set; }
    public SpeakerRequestId? CurrentSpeakerId { get; private set; }

    // Method to add a speaker request to the queue
    public SpeakerRequest AddSpeakerRequest(MeetingAttendee attendee)
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
            RequestedTime = DateTimeOffset.UtcNow,
            Status = SpeakerRequestStatus.Pending
        };

        _speakerQueue.Add(request);

        return request;
    }

    // Method to remove a speaker request from the queue
    public SpeakerRequestId RemoveSpeaker(MeetingAttendee attendee)
    {
        var request = _speakerQueue.FirstOrDefault(s => s.AttendeeId == attendee.Id);
        if (request == null)
        {
            throw new InvalidOperationException("Speaker request not found for the specified attendee.");
        }

        _speakerQueue.Remove(request);

        // Reset the current speaker if they are removed
        if (CurrentSpeaker?.AttendeeId == attendee.Id)
        {
            MoveToNextSpeaker();
        }

        return request.Id;
    }

    // Method to advance to the next speaker in the queue
    public SpeakerRequest? MoveToNextSpeaker()
    {
        if (_speakerQueue.Count == 0)
        {
            CurrentSpeaker = null;
            State = DiscussionState.Completed;
        }
        else
        {
            // Mark the current speaker as completed
            if (CurrentSpeaker != null)
            {
                CurrentSpeaker.Status = SpeakerRequestStatus.Completed;
            }

            // Skip completed speakers and set the next speaker as the current speaker
            CurrentSpeaker = _speakerQueue
                .OrderBy(x => x.RequestedTime)
                .FirstOrDefault(s => s.Status == SpeakerRequestStatus.Pending);

            if (CurrentSpeaker != null)
            {
                CurrentSpeaker.Status = SpeakerRequestStatus.InProgress;
            }

            if (CurrentSpeaker == null)
            {
                State = DiscussionState.Completed;
            }
        }

        return CurrentSpeaker;
    }

    // Method to start the session
    public void StartSession()
    {
        if (State != DiscussionState.NotStarted)
        {
            throw new InvalidOperationException("Session has already started.");
        }

        State = DiscussionState.InProgress;
    }

    // Method to manually end the session
    public void EndSession()
    {
        if (State == DiscussionState.Completed)
        {
            throw new InvalidOperationException("Session is already completed.");
        }

        // You can optionally mark all speakers as completed
        foreach (var speaker in _speakerQueue)
        {
            if (speaker.Status != SpeakerRequestStatus.Completed)
            {
                speaker.Status = SpeakerRequestStatus.Completed;
            }
        }

        // End the session manually
        State = DiscussionState.Completed;
    }

    // Method to pause the session
    public void PauseSession()
    {
        if (State != DiscussionState.InProgress)
        {
            throw new InvalidOperationException("Session is not in progress and cannot be paused.");
        }

        State = DiscussionState.Paused;
    }

    // Method to resume the session if paused
    public void ResumeSession()
    {
        if (State != DiscussionState.Paused)
        {
            throw new InvalidOperationException("Session is not paused and cannot be resumed.");
        }

        State = DiscussionState.InProgress;
    }

    // Method to reset the speaker session and clear the queue
    public void Reset()
    {
        _speakerQueue.Clear();
        CurrentSpeaker = null;
        State = DiscussionState.NotStarted;
    }

    public User? CreatedBy { get; set; } = null!;
    public UserId? CreatedById { get; set; } = null!;
    public DateTimeOffset Created { get; set; }
    public User? LastModifiedBy { get; set; }
    public UserId? LastModifiedById { get; set; }
    public DateTimeOffset? LastModified { get; set; }
}
