using System.ComponentModel.DataAnnotations;

using YourBrand.Ticketing;

public class TicketDetailsForm
{
    public int? Id { get; set; }

    [Required]
    [MaxLength(60)]
    public string Title { get; set; } = null!;

    [MaxLength(340)]
    public string? Description { get; set; }

    [Required]
    public YourBrand.Ticketing.Project? Project { get; set; }

    public YourBrand.Portal.User? Assignee { get; set; }

    public TicketStatus Status { get; set; }

    public TimeSpan? EstimatedTime { get; set; }

    public TimeSpan? CompletedTime { get; set; }

    public TimeSpan? RemainingTime { get; set; }

    public TicketPriority? Priority { get; set; }

    public TicketImpact? Impact { get; set; }

    public TicketUrgency? Urgency { get; set; }

    public void Populate(Ticket ticket)
    {
        Project = ticket.Project;
        Title = ticket.Subject;
        Description = ticket.Description;
        Status = ticket.Status;
        Assignee = ticket.Assignee?.ToUser();
        EstimatedTime = ticket.EstimatedTime;
        CompletedTime = ticket.CompletedTime;
        RemainingTime = ticket.RemainingTime;
        Priority = ticket.Priority;
        Impact = ticket.Impact;
        Urgency = ticket.Urgency;
    }
}