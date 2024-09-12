using YourBrand.Identity;
using YourBrand.Tenancy;
using YourBrand.Domain;
using YourBrand.Ticketing.Domain.Enums;
using YourBrand.Ticketing.Domain.Events;
using YourBrand.Ticketing.Domain.ValueObjects;

namespace YourBrand.Ticketing.Domain.Entities;

public class Ticket : AggregateRoot<TicketId>, IHasTenant, IHasOrganization
{
    protected Ticket()
    {
    }

    public Ticket(int id, string subject, string? description)
    {
        Id = id;
        Subject = subject;
        Description = description;
    }

    public TenantId TenantId { get; set; }

    public OrganizationId OrganizationId { get; set; }

    public string Subject { get; set; } = default!;

    public string? Description { get; }

    public TicketParticipant? ReportedBy { get; set; }

    public TicketParticipantId? ReportedById { get; set; }

    public bool UpdateSubject(string subject)
    {
        var oldSubject = Subject;
        if (subject != oldSubject)
        {
            Subject = subject;

            AddDomainEvent(new TicketUpdated(TenantId, OrganizationId, Id));
            AddDomainEvent(new TicketSubjectUpdated(TenantId, OrganizationId, Id, Subject));

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

            AddDomainEvent(new TicketUpdated(TenantId, OrganizationId, Id));
            AddDomainEvent(new TicketStatusUpdated(TenantId, OrganizationId, Id, status, oldStatus));

            return true;
        }

        return false;
    }

    public TicketParticipant? Assignee { get; set; } = null!;

    public TicketParticipantId? AssigneeId { get; set; }

    public bool UpdateAssigneeId(string? userId)
    {
        var oldAssigneeId = AssigneeId;
        if (userId != oldAssigneeId)
        {
            AssigneeId = userId;
            //AddDomainEvent(new TicketAssigneeUpdated(TenantId, OrganizationId, Id, userId, oldAssigneeId));

            return true;
        }

        return false;
    }

    public DateTime? LastMessage { get; set; } = null!;

    public string? Text { get; set; } = null!;

    public bool UpdateText(string subject)
    {
        var oldText = Text;
        if (subject != oldText)
        {
            Text = subject;

            AddDomainEvent(new TicketUpdated(TenantId, OrganizationId, Id));
            AddDomainEvent(new TicketTextUpdated(TenantId, OrganizationId, Id, Text));

            return true;
        }

        return false;
    }


    public TicketType Type { get; set; } = null!;

    public int TypeId { get; set; } = 1;

    public TicketCategory Category { get; set; } = null!;

    public int CategoryId { get; set; } = 1;

    public TicketPriority Priority { get; set; }

    public TicketUrgency Urgency { get; set; }

    public TicketImpact Impact { get; set; }

    public double? EstimatedHours { get; private set; }

    public bool UpdateEstimatedHours(double? hours)
    {
        var oldHours = EstimatedHours;
        if (hours != oldHours)
        {
            EstimatedHours = hours;

            AddDomainEvent(new TicketUpdated(TenantId, OrganizationId, Id));
            AddDomainEvent(new TicketEstimatedHoursUpdated(TenantId, OrganizationId, Id, hours, oldHours));

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

            AddDomainEvent(new TicketUpdated(TenantId, OrganizationId, Id));
            AddDomainEvent(new TicketRemainingHoursUpdated(TenantId, OrganizationId, Id, hours, oldHours));

            return true;
        }

        return false;
    }

    public HashSet<TicketParticipant> Participants { get; } = new HashSet<TicketParticipant>();

    public HashSet<TicketTag> Tags { get; } = new HashSet<TicketTag>();
    
    public HashSet<Attachment> Attachments { get; } = new HashSet<Attachment>();

    public HashSet<TicketComment> Comments { get; } = new HashSet<TicketComment>();

    // ...

    public TicketParticipant? CreatedBy { get; set; } = null!;

    public TicketParticipantId? CreatedById { get; set; } = null!;

    public DateTimeOffset Created { get; set; }

    public TicketParticipant? LastModifiedBy { get; set; }

    public TicketParticipantId? LastModifiedById { get; set; }

    public DateTimeOffset? LastModified { get; set; }
}