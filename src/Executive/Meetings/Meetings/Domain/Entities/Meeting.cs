using YourBrand.Auditability;
using YourBrand.Domain;
using YourBrand.Identity;
using YourBrand.Meetings.Domain.ValueObjects;
using YourBrand.Tenancy;

namespace YourBrand.Meetings.Domain.Entities;

public enum MeetingState
{
    Draft,
    Scheduled,
    InProgress,
    Completed,
    Canceled
}

public enum AttendeeAccessLevel
{
    Everyone,
    Participants,
    Select
}

public class Meeting : AggregateRoot<MeetingId>, IAuditableEntity<MeetingId>, IHasTenant, IHasOrganization
{
    readonly HashSet<MeetingAttendee> _attendees = new HashSet<MeetingAttendee>();

    protected Meeting()
    {
    }

    public Meeting(MeetingId id, string title) : base(id)
    {
        Title = title;
    }

    public TenantId TenantId { get; set; }
    public OrganizationId OrganizationId { get; set; }

    public string Title { get; set; }
    public DateTimeOffset ScheduledAt { get; set; }
    public string Location { get; set; }
    public string? Description { get; set; }
    public MeetingState State { get; set; } = MeetingState.Draft;
    public IReadOnlyCollection<MeetingAttendee> Attendees => _attendees;
    public Agenda? Agenda { get; set; }
    public int? CurrentAgendaItemIndex { get; private set; } = null;
    public int? CurrentAgendaSubItemIndex { get; private set; } = null;
    public Quorum Quorum { get; set; } = new Quorum();

    public AttendeeAccessLevel SpeakingRightsAccessLevel { get; set; } = AttendeeAccessLevel.Participants;
    public AttendeeAccessLevel VotingRightsAccessLevel { get; set; } = AttendeeAccessLevel.Participants;

    public bool CanAnyoneJoin { get; set; } = false;
    public AttendeeRole JoinAs { get; set; } = AttendeeRole.Observer;

    public Minutes? Minutes { get; set; }

    public DateTimeOffset? StartedAt { get; set; }
    public DateTimeOffset? CanceledAt { get; set; }
    public DateTimeOffset? EndedAt { get; set; }

    public bool IsQuorumMet()
    {
        if (Quorum.RequiredNumber == 0)
        {
            throw new InvalidOperationException("Quorum can't be zero");
        }


        int presentAttendees = 0;

        if (VotingRightsAccessLevel == AttendeeAccessLevel.Everyone)
        {
            presentAttendees = Attendees.Count(p => p.IsPresent);
        }
        else
        {
            presentAttendees = Attendees.Count(p => p.IsPresent && p.HasVotingRights.GetValueOrDefault());
        }
        return presentAttendees >= Quorum.RequiredNumber;
    }

    public void StartMeeting()
    {
        if (State != MeetingState.Scheduled)
        {
            throw new InvalidOperationException("Meeting cannot be started.");
        }

        if (!Attendees.Any())
        {
            throw new InvalidOperationException("Cannot start a meeting without attendees.");
        }

        if (Agenda == null || !Agenda.Items.Any())
        {
            throw new InvalidOperationException("Cannot start a meeting without agenda items.");
        }

        /*
        if (!IsQuorumMet())
        {
            throw new InvalidOperationException("Quorum not met.");
        }
        */

        StartedAt = DateTimeOffset.UtcNow;

        State = MeetingState.InProgress;

        CurrentAgendaItemIndex = 0;
    }

    public void CancelMeeting()
    {
        if (State != MeetingState.Scheduled && State != MeetingState.InProgress)
        {
            throw new InvalidOperationException("Only scheduled or in-progress meetings can be canceled.");
        }

        CanceledAt = DateTimeOffset.UtcNow;

        State = MeetingState.Canceled;

        var agendaItems = Agenda.Items.OrderBy(ai => ai.Order).ToList();

        foreach (var item in agendaItems.Skip(CurrentAgendaItemIndex.GetValueOrDefault()))
        {
            item.Cancel();
        }

        CurrentAgendaItemIndex = null;
    }

    public void EndMeeting()
    {
        if (State != MeetingState.InProgress)
        {
            throw new InvalidOperationException("Meeting is not in progress.");
        }

        EndedAt = DateTimeOffset.UtcNow;

        State = MeetingState.Completed;

        var remainingItems = Agenda.Items
            .OrderBy(ai => ai.Order)
            .Skip(CurrentAgendaItemIndex.GetValueOrDefault() + 1);

        foreach (var item in remainingItems)
        {
            item.State = AgendaItemState.Skipped;
        }

        CurrentAgendaItemIndex = null;
    }

    public AgendaItem MoveToNextAgendaItem()
    {
        if (Agenda is null)
        {
            throw new InvalidOperationException("No agenda is set.");
        }

        if (State != MeetingState.InProgress)
        {
            throw new InvalidOperationException("Meeting is not in progress.");
        }

        var agendaItems = Agenda.Items.OrderBy(ai => ai.Order).ToList();

        // Start from the first agenda item if index is null
        if (CurrentAgendaItemIndex == null)
        {
            CurrentAgendaItemIndex = 0;
            return agendaItems[CurrentAgendaItemIndex.Value];
        }

        var currentItem = agendaItems[CurrentAgendaItemIndex.Value];

        // Check if the current main item has sub-items
        if (currentItem.SubItems.Any())
        {
            var subItems = currentItem.SubItems.OrderBy(si => si.Order).ToList();

            // Process the main item first if sub-items haven't started
            if (CurrentAgendaSubItemIndex == null)
            {
                CurrentAgendaSubItemIndex = 0; // Start processing sub-items after the main item
                return currentItem; // Return the main item first
            }

            // Process each sub-item in order
            if (CurrentAgendaSubItemIndex < subItems.Count)
            {
                var subItem = subItems[CurrentAgendaSubItemIndex.Value];
                CurrentAgendaSubItemIndex++;

                // After the last sub-item, reset `CurrentAgendaSubItemIndex` and increment `CurrentAgendaItemIndex`
                if (CurrentAgendaSubItemIndex >= subItems.Count)
                {
                    CurrentAgendaSubItemIndex = null;
                    CurrentAgendaItemIndex++; // Move to the next main item
                }

                return subItem;
            }
        }
        else
        {
            // If there are no sub-items, directly move to the next main agenda item
            if (CurrentAgendaItemIndex < agendaItems.Count - 1)
            {
                CurrentAgendaItemIndex++;
                return agendaItems[CurrentAgendaItemIndex.Value];
            }
        }

        throw new InvalidOperationException("No more items in the agenda.");
    }

    public void ProcessCurrentAgendaItem()
    {
        var currentItem = GetCurrentAgendaItem();
        if (currentItem == null)
        {
            throw new InvalidOperationException("No current agenda item.");
        }

        if (currentItem.Type == AgendaItemType.RollCall)
        {
            // Logic to process roll call, like marking attendees present.
            Console.WriteLine("Processing Roll Call...");
        }
        else if (currentItem.Type == AgendaItemType.QuorumCheck)
        {
            if (!IsQuorumMet())
                throw new InvalidOperationException("Quorum not met.");
            Console.WriteLine("Quorum confirmed.");
        }
        else if (currentItem.Type == AgendaItemType.Motion)
        {
            // StartDiscussion();
        }
        else if (currentItem.Type == AgendaItemType.Election)
        {
            // StartVoting();
        }
        else
        {
            throw new InvalidOperationException("No handler found for this agenda item type.");
        }
    }

    public AgendaItem? GetCurrentAgendaItem()
    {
        if (CurrentAgendaItemIndex is null || Agenda == null)
        {
            return null;
        }

        var agendaItems = Agenda.Items.OrderBy(ai => ai.Order).ToList();
        var currentItem = agendaItems.ElementAtOrDefault(CurrentAgendaItemIndex.Value);

        if (currentItem == null)
        {
            return null;
        }

        // If we are processing sub-items, return the current sub-item
        if (CurrentAgendaSubItemIndex.HasValue && CurrentAgendaSubItemIndex >= 0)
        {
            var subItems = currentItem.SubItems.OrderBy(si => si.Order).ToList();
            return subItems.ElementAtOrDefault(CurrentAgendaSubItemIndex.Value - 1); // Adjust for sub-item index
        }

        // Otherwise, return the main item
        return currentItem;
    }


    public AgendaItem? GetAgendaItem(string id)
    {
        return Agenda?.Items.FirstOrDefault(x => x.Id == id);
    }

    public MeetingAttendee AddAttendee(string name, string? userId, string email, AttendeeRole role, bool? hasSpeakingRights, bool? hasVotingRights,
        MeetingGroupId? meetingGroupId = null, MeetingGroupMemberId? meetingGroupMemberId = null)
    {
        if (_attendees.Any(a => (a.UserId != null && a.UserId == userId) || a.Email.Equals(email, StringComparison.OrdinalIgnoreCase)))
        {
            throw new InvalidOperationException("An attendee with the same user ID or email already exists.");
        }

        int order = 1;

        try
        {
            var last = _attendees.OrderByDescending(x => x.Order).First();
            order = last.Order + 1;
        }
        catch { }

        var attendee = new MeetingAttendee
        {
            OrganizationId = OrganizationId,
            MeetingId = Id,
            Name = name,
            UserId = userId,
            Email = email,
            Role = role,
            HasSpeakingRights = hasSpeakingRights,
            HasVotingRights = hasVotingRights,
            MeetingGroupId = meetingGroupId,
            MeetingGroupMemberId = meetingGroupMemberId,
            AddedAt = DateTimeOffset.UtcNow
        };
        attendee.Order = order;

        _attendees.Add(attendee);

        return attendee;
    }

    public bool RemoveAttendee(MeetingAttendee attendee)
    {

        attendee.RemovedAt = DateTimeOffset.UtcNow;
        return _attendees.Remove(attendee);
    }

    public MeetingAttendee? GetAttendeeById(string id)
    {
        return Attendees.FirstOrDefault(x => x.Id == id);
    }

    public MeetingAttendee? GetAttendeeByUserId(string userId)
    {
        return Attendees.FirstOrDefault(x => x.UserId == userId);
    }

    public void ResetMeetingProgress()
    {
        State = MeetingState.Scheduled;

        CurrentAgendaItemIndex = null;

        foreach (var agendaItem in Agenda!.Items)
        {
            agendaItem.State = AgendaItemState.Pending;
        }
    }

    public bool IsAttendeeAllowedToSpeak(MeetingAttendee attendee)
    {
        if (SpeakingRightsAccessLevel == AttendeeAccessLevel.Everyone)
        {
            return true;
        }

        if (SpeakingRightsAccessLevel == AttendeeAccessLevel.Participants && attendee.Role != AttendeeRole.Observer)
        {
            if (attendee.HasSpeakingRights is null)
            {
                return true;
            }

            return attendee.HasSpeakingRights.GetValueOrDefault();
        }

        if (SpeakingRightsAccessLevel == AttendeeAccessLevel.Select)
        {
            return attendee.HasSpeakingRights.GetValueOrDefault();
        }

        return false;
    }

    public bool IsAttendeeAllowedToVote(MeetingAttendee attendee)
    {
        if (VotingRightsAccessLevel == AttendeeAccessLevel.Everyone)
        {
            return true;
        }

        if (VotingRightsAccessLevel == AttendeeAccessLevel.Participants && attendee.Role != AttendeeRole.Observer)
        {
            if (attendee.HasVotingRights is null)
            {
                return true;
            }

            return attendee.HasVotingRights.GetValueOrDefault();
        }

        if (VotingRightsAccessLevel == AttendeeAccessLevel.Select)
        {
            return attendee.HasVotingRights.GetValueOrDefault();
        }

        return false;
    }

    // Determines if the meeting can be started
    public bool CanStart =>
        State == MeetingState.Scheduled &&
        Attendees.Any() &&
        Agenda != null && Agenda.Items.Any();

    // Determines if the meeting can move to the next agenda item
    public bool CanMoveNext
    {
        get
        {
            if (State != MeetingState.InProgress || Agenda == null || !CurrentAgendaItemIndex.HasValue)
            {
                return false;
            }

            var currentItem = GetCurrentAgendaItem();
            if (currentItem == null)
            {
                return false;
            }

            // Allow moving to the next item only if the current item's state allows it
            return (currentItem.State == AgendaItemState.Completed ||
                    currentItem.State == AgendaItemState.Skipped ||
                    currentItem.State == AgendaItemState.Canceled) &&
                   CurrentAgendaItemIndex < Agenda.Items.Count - 1;
        }
    }

    // Determines if the meeting can be canceled
    public bool CanCancel =>
        State == MeetingState.Scheduled || State == MeetingState.InProgress;

    // Determines if the meeting can be ended
    public bool CanEnd
    {
        get
        {
            if (State != MeetingState.InProgress || Agenda == null)
            {
                return false;
            }

            // Check if all items are in a terminal state
            return Agenda.Items.All(item =>
                item.State == AgendaItemState.Completed ||
                item.State == AgendaItemState.Skipped ||
                item.State == AgendaItemState.Canceled);
        }
    }

    public User? CreatedBy { get; set; } = null!;
    public UserId? CreatedById { get; set; } = null!;
    public DateTimeOffset Created { get; set; }
    public User? LastModifiedBy { get; set; }
    public UserId? LastModifiedById { get; set; }
    public DateTimeOffset? LastModified { get; set; }
}