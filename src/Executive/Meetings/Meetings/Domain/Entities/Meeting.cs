using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

using YourBrand.Auditability;
using YourBrand.Domain;
using YourBrand.Identity;
using YourBrand.Meetings.Domain.Functions;
using YourBrand.Meetings.Domain.ValueObjects;
using YourBrand.Tenancy;

namespace YourBrand.Meetings.Domain.Entities;

public enum MeetingState
{
    Draft,
    Scheduled,
    InProgress,
    Adjourned,
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
    public DateTimeOffset? AdjournedAt { get; private set; }
    public string? AdjournmentMessage { get; private set; }
    public IReadOnlyCollection<MeetingAttendee> Attendees => _attendees;
    public Agenda? Agenda { get; set; }
    public int? CurrentAgendaItemIndex { get; private set; } = null;
    public int? CurrentAgendaSubItemIndex { get; private set; } = null;
    public Quorum Quorum { get; set; } = new Quorum();

    public AttendeeAccessLevel SpeakingRightsAccessLevel { get; set; } = AttendeeAccessLevel.Participants;
    public AttendeeAccessLevel VotingRightsAccessLevel { get; set; } = AttendeeAccessLevel.Participants;

    public bool CanAnyoneJoin { get; set; } = false;
    public int JoinAsId { get; set; } = AttendeeRole.Observer.Id;
    public AttendeeRole JoinAs { get; set; } = AttendeeRole.Observer;

    public Minutes? Minutes { get; set; }

    public DateTimeOffset? StartedAt { get; set; }
    public DateTimeOffset? CanceledAt { get; set; }
    public DateTimeOffset? EndedAt { get; set; }

    public void Reschedule(DateTimeOffset scheduledAt)
    {
        ScheduledAt = scheduledAt;

        AddDomainEvent(new MeetingScheduledAtChanged(TenantId, OrganizationId, Id, ScheduledAt));
    }

    public bool ShowAgendaTimeEstimates { get; set; }

    [Throws(typeof(InvalidOperationException))]
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

    [Throws(typeof(InvalidOperationException))]
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
        ClearAdjournment();

        CurrentAgendaItemIndex = 0;
        CurrentAgendaSubItemIndex = null;

        RaiseMeetingStateChanged();

        var currentAgendaItem = GetCurrentAgendaItem();
        if (currentAgendaItem is not null)
        {
            RaiseAgendaItemChanged(currentAgendaItem.Id.Value);
        }
    }

    [Throws(typeof(InvalidOperationException))]
    public void AdjournMeeting(string message)
    {
        if (State != MeetingState.InProgress)
        {
            throw new InvalidOperationException("Only in-progress meetings can be adjourned.");
        }

        if (string.IsNullOrWhiteSpace(message))
        {
            throw new InvalidOperationException("Adjournment message is required.");
        }

        AdjournedAt = DateTimeOffset.UtcNow;
        AdjournmentMessage = message;
        State = MeetingState.Adjourned;

        RaiseMeetingStateChanged();
    }

    [Throws(typeof(InvalidOperationException))]
    public void ResumeMeeting()
    {
        if (State != MeetingState.Adjourned)
        {
            throw new InvalidOperationException("Only adjourned meetings can be resumed.");
        }

        State = MeetingState.InProgress;
        ClearAdjournment();

        RaiseMeetingStateChanged();
    }

    public void ClearAdjournment()
    {
        AdjournmentMessage = null;
        AdjournedAt = null;
    }

    [Throws(typeof(InvalidOperationException))]
    public void CancelMeeting()
    {
        if (State != MeetingState.Scheduled && State != MeetingState.InProgress && State != MeetingState.Adjourned)
        {
            throw new InvalidOperationException("Only scheduled or in-progress meetings can be canceled.");
        }

        CanceledAt = DateTimeOffset.UtcNow;

        State = MeetingState.Canceled;
        ClearAdjournment();

        // Skip remaining agenda items and their sub-items
        var remainingItems = Agenda.Items
            .OrderBy(ai => ai.Order)
            .Skip(CurrentAgendaItemIndex.GetValueOrDefault() + 1);

        foreach (var item in remainingItems)
        {
            item.Cancel();

            foreach (var subItem in item.SubItems.OrderBy(si => si.Order))
            {
                subItem.Cancel();
            }
        }

        RaiseMeetingStateChanged();
    }

    [Throws(typeof(InvalidOperationException))]
    public void EndMeeting()
    {
        if (State != MeetingState.InProgress)
        {
            throw new InvalidOperationException("Meeting is not in progress.");
        }

        EndedAt = DateTimeOffset.UtcNow;
        State = MeetingState.Completed;
        ClearAdjournment();

        // Skip remaining agenda items and their sub-items
        var remainingItems = Agenda.Items
            .OrderBy(ai => ai.Order)
            .Skip(CurrentAgendaItemIndex.GetValueOrDefault() + 1);

        foreach (var item in remainingItems)
        {
            item.Skip();

            foreach (var subItem in item.SubItems.OrderBy(si => si.Order))
            {
                subItem.Skip();
            }
        }

        RaiseMeetingStateChanged();
    }

    [Throws(typeof(InvalidOperationException))]
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

        AgendaItem? result = null;

        // Start from the first agenda item if index is null
        if (CurrentAgendaItemIndex == null)
        {
            if (!agendaItems.Any())
            {
                throw new InvalidOperationException("No agenda items available.");
            }

            CurrentAgendaItemIndex = 0;
            result = agendaItems[CurrentAgendaItemIndex.Value];
        }
        else if (CurrentAgendaItemIndex < 0 || CurrentAgendaItemIndex >= agendaItems.Count)
        {
            throw new InvalidOperationException("No more items in the agenda.");
        }
        else
        {
            var currentItem = agendaItems[CurrentAgendaItemIndex.Value];

            // Check if the current main item has sub-items
            if (currentItem.SubItems.Any())
            {
                var subItems = currentItem.SubItems.OrderBy(si => si.Order).ToList();

                // Process the main item first if sub-items haven't started
                if (CurrentAgendaSubItemIndex == null)
                {
                    CurrentAgendaSubItemIndex = 0; // Start processing sub-items after the main item
                    result = currentItem; // Return the main item first
                }
                else if (CurrentAgendaSubItemIndex < subItems.Count)
                {
                    var subItem = subItems[CurrentAgendaSubItemIndex.Value];
                    CurrentAgendaSubItemIndex++;

                    // After the last sub-item, reset `CurrentAgendaSubItemIndex` and increment `CurrentAgendaItemIndex`
                    if (CurrentAgendaSubItemIndex >= subItems.Count)
                    {
                        CurrentAgendaSubItemIndex = null;
                        CurrentAgendaItemIndex++; // Move to the next main item
                    }

                    result = subItem;
                }
            }
            else if (CurrentAgendaItemIndex < agendaItems.Count - 1)
            {
                // If there are no sub-items, directly move to the next main agenda item
                CurrentAgendaItemIndex++;
                result = agendaItems[CurrentAgendaItemIndex.Value];
            }
        }

        if (result is null)
        {
            throw new InvalidOperationException("No more items in the agenda.");
        }

        if (result.State == AgendaItemState.Pending || result.State == AgendaItemState.Postponed)
        {
            result.Activate();
        }

        RaiseAgendaItemChanged(result.Id.Value);

        return result;
    }

    [Throws(typeof(InvalidOperationException))]
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
        }
        else if (currentItem.Type == AgendaItemType.QuorumCheck)
        {
            if (!IsQuorumMet())
                throw new InvalidOperationException("Quorum not met.");
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
        var currentItem = agendaItems.ElementAtOrDefault(CurrentAgendaItemIndex.GetValueOrDefault());

        if (currentItem == null)
        {
            return null;
        }

        // If we are processing sub-items, return the current sub-item directly
        if (CurrentAgendaSubItemIndex.HasValue)
        {
            var subItems = currentItem.SubItems.OrderBy(si => si.Order).ToList();
            return subItems.ElementAtOrDefault(CurrentAgendaSubItemIndex.GetValueOrDefault()); // No need for -1 adjustment
        }

        // Otherwise, return the main item
        return currentItem;
    }


    public AgendaItem? GetAgendaItem(string id)
    {
        return Agenda?.Items.FirstOrDefault(x => x.Id == id);
    }

    [Throws(typeof(InvalidOperationException))]
    public MeetingAttendee AddAttendee(string name, string? userId, string email, AttendeeRole role, bool? hasSpeakingRights, bool? hasVotingRights,
        IEnumerable<MeetingFunction>? functions = null,
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
            TenantId = TenantId,
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
        attendee.SetFunctions(functions ?? Enumerable.Empty<MeetingFunction>());

        _attendees.Add(attendee);

        return attendee;
    }

    [Throws(typeof(InvalidOperationException))]
    public async Task AddAttendeesFromGroup(MeetingGroup meetingGroup, IApplicationDbContext context, CancellationToken cancellationToken = default)
    {
        foreach (var member in meetingGroup.Members)
        {
            var role = await context.AttendeeRoles.FirstOrDefaultAsync(x => x.Id == member.Role.Id, cancellationToken);

            if (role is null)
            {
                throw new Exception("Invalid role");
            }

            var attendee = AddAttendee(member.Name, member.UserId, member.Email, role, member.HasSpeakingRights, member.HasVotingRights, functions: null, meetingGroupId: member.MeetingGroupId, meetingGroupMemberId: member.Id);
        }
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

    public IEnumerable<MeetingAttendee> GetAttendeesByFunction(MeetingFunction function)
    {
        return Attendees.Where(x => x.HasFunction(function));
    }

    public void SetOpenAccess(bool canAnyoneJoin, AttendeeRole joinAs)
    {
        if (canAnyoneJoin && joinAs != AttendeeRole.Member && joinAs != AttendeeRole.Observer)
        {
            throw new InvalidOperationException("Only member or observer roles can be used for open access.");
        }

        CanAnyoneJoin = canAnyoneJoin;
        JoinAs = joinAs;
        JoinAsId = joinAs.Id;
    }

    public void ResetMeetingProgress()
    {
        State = MeetingState.Scheduled;

        ClearAdjournment();

        CurrentAgendaItemIndex = null;
        CurrentAgendaSubItemIndex = null;

        foreach (var agendaItem in Agenda!.Items)
        {
            agendaItem.Reset();

            foreach (var subItem in agendaItem.SubItems.OrderBy(si => si.Order))
            {
                subItem.Reset();
            }
        }

        RaiseMeetingStateChanged();
    }

    public void ChangeState(MeetingState newState, string? adjournmentMessage = null)
    {
        State = newState;

        if (newState == MeetingState.Adjourned)
        {
            if (!string.IsNullOrWhiteSpace(adjournmentMessage))
            {
                AdjournmentMessage = adjournmentMessage;
            }
        }
        else
        {
            ClearAdjournment();
        }

        RaiseMeetingStateChanged();
    }

    private void RaiseMeetingStateChanged()
    {
        AddDomainEvent(new MeetingStateChanged(TenantId, OrganizationId, Id, State, AdjournmentMessage));
    }

    private void RaiseAgendaItemChanged(string? agendaItemId)
    {
        AddDomainEvent(new MeetingAgendaItemChanged(TenantId, OrganizationId, Id, agendaItemId));
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

    public bool HasFunctionAssigned(MeetingFunction function) =>
        Attendees.Any(a => a.HasFunction(function));

    public bool CanAttendeeActAsChair(MeetingAttendee attendee)
    {
        if (attendee.HasFunction(MeetingFunction.Chairperson))
        {
            return true;
        }

        if (!HasFunctionAssigned(MeetingFunction.Chairperson))
        {
            if (attendee.HasFunction(MeetingFunction.Facilitator))
            {
                return true;
            }

            if (CreatedById is not null && attendee.UserId is not null && attendee.UserId == CreatedById)
            {
                return true;
            }
        }

        return false;
    }

    public ChairpersonMeetingFunction? GetChairpersonFunction(MeetingAttendee attendee) =>
        CanAttendeeActAsChair(attendee) ? new ChairpersonMeetingFunction(this, attendee) : null;

    public void AssignFunctionToAttendee(MeetingAttendee attendee, MeetingFunction function)
    {
        foreach (var other in Attendees)
        {
            var retainedFunctions = other.Functions
                .Where(f => f.MeetingFunctionId != function.Id)
                .Select(f => f.Function ?? MeetingFunction.AllFunctions.FirstOrDefault(mf => mf.Id == f.MeetingFunctionId))
                .Where(f => f is not null)
                .Cast<MeetingFunction>()
                .ToList();

            if (other == attendee && !retainedFunctions.Any(f => f.Id == function.Id))
            {
                retainedFunctions.Add(function);
            }

            other.SetFunctions(retainedFunctions);
        }
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

            // Check if all sub-items are in a terminal state before moving to the next main item
            if (currentItem.SubItems.Any(subItem =>
                subItem.State != AgendaItemState.Completed &&
                subItem.State != AgendaItemState.Skipped &&
                subItem.State != AgendaItemState.Canceled &&
                subItem.State != AgendaItemState.Postponed))
            {
                return false;
            }

            return (currentItem.State == AgendaItemState.Completed ||
                    currentItem.State == AgendaItemState.Skipped ||
                    currentItem.State == AgendaItemState.Canceled ||
                    currentItem.State == AgendaItemState.Postponed) &&
                   CurrentAgendaItemIndex < Agenda.Items.Count - 1;
        }
    }

    // Determines if the meeting can be canceled
    public bool CanCancel =>
        State == MeetingState.Scheduled || State == MeetingState.InProgress || State == MeetingState.Adjourned;

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
                item.State == AgendaItemState.Canceled ||
                item.State == AgendaItemState.Postponed);
        }
    }

    public User? CreatedBy { get; set; } = null!;
    public UserId? CreatedById { get; set; } = null!;
    public DateTimeOffset Created { get; set; }
    public User? LastModifiedBy { get; set; }
    public UserId? LastModifiedById { get; set; }
    public DateTimeOffset? LastModified { get; set; }
}