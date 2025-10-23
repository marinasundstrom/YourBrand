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
    private TimeSpan _currentSpeakerClockAccumulated = TimeSpan.Zero;

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

    public TimeSpan? SpeakingTimeLimit { get; private set; }

    // Property to store the current speaker
    public SpeakerRequest? CurrentSpeaker { get; private set; }
    public SpeakerRequestId? CurrentSpeakerId { get; private set; }

    public bool IsCurrentSpeakerClockRunning => CurrentSpeakerClockStartedAt is not null;

    public DateTimeOffset? CurrentSpeakerClockStartedAt { get; private set; }

    public TimeSpan CurrentSpeakerClockAccumulated => _currentSpeakerClockAccumulated;

    // Method to add a speaker request to the queue
    [Throws(typeof(InvalidOperationException))]
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
            SpeakerId = Id,
            RequestedTime = DateTimeOffset.UtcNow,
            Status = SpeakerRequestStatus.Pending
        };

        request.ApplyDefaultSpeakingTime(SpeakingTimeLimit);

        _speakerQueue.Add(request);

        return request;
    }

    // Method to remove a speaker request from the queue
    [Throws(typeof(InvalidOperationException))]
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
    public SpeakerTransition MoveToNextSpeaker()
    {
        var now = DateTimeOffset.UtcNow;

        SpeakerRequest? previousSpeaker = null;
        TimeSpan? previousElapsed = null;

        if (CurrentSpeaker != null)
        {
            previousSpeaker = CurrentSpeaker;

            if (IsCurrentSpeakerClockRunning)
            {
                StopCurrentSpeakerClock(now);
            }
            else
            {
                CurrentSpeaker.ActualSpeakingTime = _currentSpeakerClockAccumulated;
            }

            previousElapsed = CurrentSpeaker.ActualSpeakingTime ?? _currentSpeakerClockAccumulated;
            CurrentSpeaker.Status = SpeakerRequestStatus.Completed;
        }

        SpeakerRequest? nextSpeaker = null;

        if (_speakerQueue.Count > 0)
        {
            nextSpeaker = _speakerQueue
                .OrderBy(x => x.RequestedTime)
                .FirstOrDefault(s => s.Status == SpeakerRequestStatus.Pending);
        }

        if (nextSpeaker != null)
        {
            CurrentSpeaker = nextSpeaker;
            CurrentSpeaker.Status = SpeakerRequestStatus.InProgress;
            CurrentSpeakerId = CurrentSpeaker.Id;
            ResetCurrentSpeakerClockState();
            State = DiscussionState.InProgress;
        }
        else
        {
            CurrentSpeaker = null;
            CurrentSpeakerId = null;
            ResetCurrentSpeakerClockState();
            State = DiscussionState.Completed;
        }

        return new SpeakerTransition(previousSpeaker, CurrentSpeaker, previousElapsed);
    }

    // Method to start the session
    [Throws(typeof(InvalidOperationException))]
    public void StartSession()
    {
        if (State != DiscussionState.NotStarted)
        {
            throw new InvalidOperationException("Session has already started.");
        }

        State = DiscussionState.InProgress;
    }

    // Method to manually end the session
    [Throws(typeof(InvalidOperationException))]
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
    [Throws(typeof(InvalidOperationException))]
    public void PauseSession()
    {
        if (State != DiscussionState.InProgress)
        {
            throw new InvalidOperationException("Session is not in progress and cannot be paused.");
        }

        State = DiscussionState.Paused;
    }

    // Method to resume the session if paused
    [Throws(typeof(InvalidOperationException))]
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
        ResetCurrentSpeakerClockState();
    }

    public void SetSpeakingTimeLimit(TimeSpan? speakingTimeLimit)
    {
        if (speakingTimeLimit.HasValue && speakingTimeLimit.Value <= TimeSpan.Zero)
        {
            throw new InvalidOperationException("Speaking time must be greater than zero.");
        }

        SpeakingTimeLimit = speakingTimeLimit;

        foreach (var speaker in _speakerQueue.Where(s => s.Status != SpeakerRequestStatus.Completed))
        {
            speaker.ApplyDefaultSpeakingTime(SpeakingTimeLimit);
        }

        if (CurrentSpeaker is not null && CurrentSpeaker.Status == SpeakerRequestStatus.InProgress)
        {
            CurrentSpeaker.ApplyDefaultSpeakingTime(SpeakingTimeLimit);
        }
    }

    [Throws(typeof(InvalidOperationException))]
    public void ExtendSpeakerTime(SpeakerRequestId speakerRequestId, TimeSpan additionalSpeakingTime)
    {
        var speakerRequest = _speakerQueue.FirstOrDefault(s => s.Id == speakerRequestId)
            ?? (CurrentSpeaker?.Id == speakerRequestId ? CurrentSpeaker : null);

        if (speakerRequest is null)
        {
            throw new InvalidOperationException("Speaker request not found for the specified attendee.");
        }

        speakerRequest.ExtendSpeakingTime(additionalSpeakingTime);
    }

    public void StartCurrentSpeakerClock(DateTimeOffset now)
    {
        if (CurrentSpeaker is null)
        {
            throw new InvalidOperationException("No current speaker to start clock for.");
        }

        if (IsCurrentSpeakerClockRunning)
        {
            throw new InvalidOperationException("Clock already running for current speaker.");
        }

        CurrentSpeakerClockStartedAt = now;
    }

    public void StopCurrentSpeakerClock(DateTimeOffset now)
    {
        if (CurrentSpeaker is null)
        {
            throw new InvalidOperationException("No current speaker to stop clock for.");
        }

        if (!IsCurrentSpeakerClockRunning)
        {
            throw new InvalidOperationException("Clock is not running for current speaker.");
        }

        _currentSpeakerClockAccumulated += now - CurrentSpeakerClockStartedAt!.Value;
        CurrentSpeakerClockStartedAt = null;
        CurrentSpeaker.ActualSpeakingTime = _currentSpeakerClockAccumulated;
    }

    public TimeSpan GetCurrentSpeakerClockElapsed(DateTimeOffset now)
    {
        var elapsed = _currentSpeakerClockAccumulated;

        if (IsCurrentSpeakerClockRunning)
        {
            elapsed += now - CurrentSpeakerClockStartedAt!.Value;
        }

        return elapsed < TimeSpan.Zero ? TimeSpan.Zero : elapsed;
    }

    public SpeakerClockSnapshot GetCurrentSpeakerClockSnapshot(DateTimeOffset now)
    {
        var elapsed = GetCurrentSpeakerClockElapsed(now);

        return new SpeakerClockSnapshot(IsCurrentSpeakerClockRunning, elapsed, CurrentSpeakerClockStartedAt);
    }

    public void ResetCurrentSpeakerClock()
    {
        if (CurrentSpeaker is null)
        {
            throw new InvalidOperationException("No current speaker to reset clock for.");
        }

        ResetCurrentSpeakerClockState();
    }

    private void ResetCurrentSpeakerClockState()
    {
        CurrentSpeakerClockStartedAt = null;
        _currentSpeakerClockAccumulated = TimeSpan.Zero;
        if (CurrentSpeaker is not null)
        {
            CurrentSpeaker.ActualSpeakingTime = null;
        }
    }

    public User? CreatedBy { get; set; } = null!;
    public UserId? CreatedById { get; set; } = null!;
    public DateTimeOffset Created { get; set; }
    public User? LastModifiedBy { get; set; }
    public UserId? LastModifiedById { get; set; }
    public DateTimeOffset? LastModified { get; set; }
}

public sealed record SpeakerTransition(SpeakerRequest? PreviousSpeaker, SpeakerRequest? CurrentSpeaker, TimeSpan? PreviousElapsed);

public sealed record SpeakerClockSnapshot(bool IsRunning, TimeSpan Elapsed, DateTimeOffset? StartedAtUtc);
