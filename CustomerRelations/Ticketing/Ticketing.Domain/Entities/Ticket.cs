using YourBrand.Ticketing.Domain.Enums;
using YourBrand.Ticketing.Domain.Events;

namespace YourBrand.Ticketing.Domain.Entities;

public class Ticket : AggregateRoot<int>, IAuditable
{
    protected Ticket()
    {
    }

    public Ticket(string subject, string requester, string description)
    {
        Subject = subject;
        Requester = requester;
        Description = description;
    }

    public string Requester { get; set; } = "Test";
    public string Description { get; } = null!;
    public string Subject { get; set; } = ""; // null!;

    public bool UpdateSubject(string title)
    {
        var oldTitle = Subject;
        if (title != oldTitle)
        {
            Subject = title;

            AddDomainEvent(new TicketUpdated(Id));
            AddDomainEvent(new TicketSubjectUpdated(Id, Subject));

            return true;
        }

        return false;
    }

    public TicketStatus Status { get; set; } = null!;

    public int StatusId { get; set; } = 1;

    public bool UpdateStatus(TicketStatus status)
    {
        var oldStatus = Status;
        if (status != oldStatus)
        {
            Status = status;

            AddDomainEvent(new TicketUpdated(Id));
            AddDomainEvent(new TicketStatusUpdated(Id, status, oldStatus));

            return true;
        }

        return false;
    }

    public User? Assignee { get; set; } = null!;

    public string? AssigneeId { get; set; }

    public bool UpdateAssigneeId(string? userId)
    {
        var oldAssigneeId = AssigneeId;
        if (userId != oldAssigneeId)
        {
            AssigneeId = userId;
            AddDomainEvent(new TicketAssignedUserUpdated(Id, userId, oldAssigneeId));

            return true;
        }

        return false;
    }

    public DateTime? LastMessage { get; set; } = null!;

    public string? Text { get; set; } = null!;

    public bool UpdateText(string title)
    {
        var oldText = Text;
        if (title != oldText)
        {
            Text = title;

            AddDomainEvent(new TicketUpdated(Id));
            AddDomainEvent(new TicketTextUpdated(Id, Text));

            return true;
        }

        return false;
    }


    public TicketType Type { get; set; } = null!;

    public int TypeId { get; set; } = 1;

    public TicketPriority Priority { get; set; }

    public TicketSeverity Severity { get; set; }

    public TicketImpact Impact { get; set; }

    public double? EstimatedHours { get; private set; }

    public bool UpdateEstimatedHours(double? hours)
    {
        var oldHours = EstimatedHours;
        if (hours != oldHours)
        {
            EstimatedHours = hours;

            AddDomainEvent(new TicketUpdated(Id));
            AddDomainEvent(new TicketEstimatedHoursUpdated(Id, hours, oldHours));

            return true;
        }

        return false;
    }

    public double? RemainingHours { get; private set; }

    public bool UpdateRemainingHours(double? hours)
    {
        var oldHours = RemainingHours;
        if (hours != oldHours)
        {
            RemainingHours = hours;

            AddDomainEvent(new TicketUpdated(Id));
            AddDomainEvent(new TicketRemainingHoursUpdated(Id, hours, oldHours));

            return true;
        }

        return false;
    }

    public HashSet<Tag> Tags { get; } = new HashSet<Tag>();

    public HashSet<Attachment> Attachments { get; } = new HashSet<Attachment>();

    public HashSet<TicketComment> Comments { get; } = new HashSet<TicketComment>();

    // ...

    public User? CreatedBy { get; set; } = null!;

    public string? CreatedById { get; set; } = null!;

    public DateTimeOffset Created { get; set; }

    public User? LastModifiedBy { get; set; }

    public string? LastModifiedById { get; set; }

    public DateTimeOffset? LastModified { get; set; }
}
