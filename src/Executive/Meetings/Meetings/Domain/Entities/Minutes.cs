using YourBrand.Auditability;
using YourBrand.Domain;
using YourBrand.Identity;
using YourBrand.Meetings.Domain.ValueObjects;
using YourBrand.Tenancy;

namespace YourBrand.Meetings.Domain.Entities;

public enum MinutesState
{
    Draft,            // Minute is being drafted and can still be modified.
    UnderReview,      // Minute is under review by attendees or stakeholders.
    Approved          // Minute is finalized and approved for the meeting.
}

public class Minutes : AggregateRoot<MinutesId>, IAuditableEntity<MinutesId>, IHasTenant, IHasOrganization
{
    readonly HashSet<MinutesItem> _items = new HashSet<MinutesItem>();
    readonly HashSet<MinutesAttendee> _attendees = new HashSet<MinutesAttendee>();
    readonly HashSet<MinutesTask> _tasks = new HashSet<MinutesTask>();

    // Apologies / Absentees

    protected Minutes()
    {
    }

    public Minutes(int id) : base(id)
    {

    }

    public TenantId TenantId { get; set; }
    public OrganizationId OrganizationId { get; set; }
    public MeetingId MeetingId { get; set; }


    public string Title { get; set; }
    public DateTimeOffset? Date { get; set; } = DateTimeOffset.UtcNow;
    public string Location { get; set; }
    public string? Description { get; set; }

    public MinutesState State { get; set; } = MinutesState.Draft;

    public Agenda? Agenda { get; set; }

    public IReadOnlyCollection<MinutesAttendee> Attendees => _attendees;

    public IReadOnlyCollection<MinutesTask> Tasks => _tasks;

    public MinutesAttendee AddAttendee(string name, string? userId, string email, AttendeeRole role, bool? hasSpeakingRights, bool? hasVotingRights,
        MeetingGroupId? meetingGroupId = null, MeetingGroupMemberId? meetingGroupMemberId = null)
    {
        int order = 1;

        try
        {
            var last = _attendees.OrderByDescending(x => x.Order).First();
            order = last.Order + 1;
        }
        catch { }

        var attendee = new MinutesAttendee
        {
            OrganizationId = OrganizationId,
            MinutesId = Id,
            Name = name,
            UserId = userId,
            Email = email,
            Role = role,
            HasSpeakingRights = hasSpeakingRights,
            HasVotingRights = hasVotingRights,
            MeetingGroupId = meetingGroupId,
            MeetingGroupMemberId = meetingGroupMemberId
        };
        attendee.Order = order;

        _attendees.Add(attendee);

        return attendee;
    }

    public bool RemoveAttendee(MinutesAttendee attendee)
    {
        return _attendees.Remove(attendee);
    }

    public MinutesTask EnsureTask(
        MinutesTaskType type,
        MeetingAttendee attendee,
        DateTimeOffset? dueAt,
        string title,
        string? description)
    {
        if (attendee.UserId is null)
        {
            throw new InvalidOperationException("Cannot assign a minutes task to an attendee without a linked user.");
        }

        var existing = _tasks
            .FirstOrDefault(x => x.Type == type && x.AssignedToId == attendee.UserId && x.Status != MinutesTaskStatus.Cancelled);

        if (existing is not null)
        {
            existing.DueAt = dueAt;
            existing.Title = title;
            existing.Description = description;
            return existing;
        }

        var task = new MinutesTask
        {
            TenantId = TenantId,
            OrganizationId = OrganizationId,
            MinutesId = Id,
            Type = type,
            Title = title,
            Description = description,
            AssignedToId = attendee.UserId,
            AssignedToName = attendee.Name ?? attendee.Email ?? attendee.UserId?.ToString(),
            AssignedToEmail = attendee.Email,
            DueAt = dueAt,
            AssignedAt = DateTimeOffset.UtcNow
        };

        _tasks.Add(task);

        return task;
    }

    public void RemoveTasksForAssignee(MeetingAttendee attendee, MinutesTaskType type)
    {
        var toRemove = _tasks.Where(x => x.Type == type && x.AssignedToId == attendee.UserId).ToList();

        foreach (var item in toRemove)
        {
            _tasks.Remove(item);
        }
    }

    public void EnsurePostMeetingWorkflowTasks(Meeting meeting)
    {
        if (meeting.EndedAt is null)
        {
            return;
        }

        var endedAt = meeting.EndedAt.Value;

        var adjusters = meeting.GetAttendeesByFunction(MeetingFunction.MinuteAdjuster)
            .Where(x => x.UserId is not null)
            .ToList();

        foreach (var adjuster in adjusters)
        {
            EnsureTask(
                MinutesTaskType.AdjustMinutes,
                adjuster,
                endedAt.AddDays(2),
                $"Adjust minutes for {meeting.Title}",
                "Review and refine the minutes to ensure accuracy before approvals.");
        }

        var approvers = meeting.GetAttendeesByFunction(MeetingFunction.Chairperson)
            .Concat(meeting.GetAttendeesByFunction(MeetingFunction.Secretary))
            .Where(x => x.UserId is not null)
            .DistinctBy(x => x.UserId)
            .ToList();

        foreach (var approver in approvers)
        {
            EnsureTask(
                MinutesTaskType.ApproveMinutes,
                approver,
                endedAt.AddDays(5),
                $"Approve minutes for {meeting.Title}",
                "Review and approve the finalized meeting minutes.");
        }

        if (_tasks.Any())
        {
            State = MinutesState.UnderReview;
        }
    }

    public void UpdateStateFromTasks()
    {
        var activeTasks = _tasks.Where(x => x.Status != MinutesTaskStatus.Cancelled).ToList();

        if (!activeTasks.Any())
        {
            State = MinutesState.Draft;
            return;
        }

        if (activeTasks.All(x => x.Status == MinutesTaskStatus.Completed))
        {
            State = MinutesState.Approved;
        }
        else
        {
            State = MinutesState.UnderReview;
        }
    }

    public DateTimeOffset? Started { get; set; }
    public DateTimeOffset? Canceled { get; set; }
    public DateTimeOffset? Ended { get; set; }

    /*
    public bool? IsApproved { get; set; }
    public DateTimeOffset? ApprovedAt { get; set; }
    public bool? IsAdjusted { get; set; }
    public DateTimeOffset? AdjustedAt { get; set; }
    public bool? IsCompleted { get; set; }
    public DateTimeOffset? CompletedAt { get; set; }
    */

    public IReadOnlyCollection<MinutesItem> Items => _items;

    public MinutesItem AddItem(AgendaItemType type, AgendaId? agendaId, string? agendaItemId, string title, string description)
    {
        int order = 1;

        try
        {
            var last = _items.OrderByDescending(x => x.Order).First();
            order = last.Order + 1;
        }
        catch { }

        var item = new MinutesItem(type, title, description);
        item.AgendaId = agendaId;
        item.AgendaItemId = agendaItemId;
        item.Order = order;
        _items.Add(item);
        return item;
    }

    public bool RemoveItem(MinutesItem item)
    {
        return _items.Remove(item);
    }

    public bool ReorderItem(MinutesItem minuteItem, int newOrderPosition)
    {
        if (!_items.Contains(minuteItem))
        {
            throw new InvalidOperationException("Item does not exist in this group.");
        }

        if (newOrderPosition < 1 || newOrderPosition > _items.Count)
        {
            throw new ArgumentOutOfRangeException(nameof(newOrderPosition), "New order position is out of range.");
        }

        int oldOrderPosition = minuteItem.Order;

        if (oldOrderPosition == newOrderPosition)
            return false;

        // Flyttar objektet uppåt i listan
        if (newOrderPosition < oldOrderPosition)
        {
            var itemsToIncrement = Items
                .Where(i => i.Order >= newOrderPosition && i.Order < oldOrderPosition)
                .ToList();

            foreach (var item in itemsToIncrement)
            {
                item.Order += 1;
            }
        }
        // Flyttar objektet nedåt i listan
        else
        {
            var itemsToDecrement = Items
                .Where(i => i.Order > oldOrderPosition && i.Order <= newOrderPosition)
                .ToList();

            foreach (var item in itemsToDecrement)
            {
                item.Order -= 1;
            }
        }

        // Uppdatera order för objektet som flyttas
        minuteItem.Order = newOrderPosition;

        return true;
    }

    public User? CreatedBy { get; set; } = null!;
    public UserId? CreatedById { get; set; } = null!;
    public DateTimeOffset Created { get; set; }
    public User? LastModifiedBy { get; set; }
    public UserId? LastModifiedById { get; set; }
    public DateTimeOffset? LastModified { get; set; }
}