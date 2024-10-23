namespace YourBrand.Ticketing.Domain;

using YourBrand.Ticketing.Domain.Enums;

public sealed class TicketPriorityCalculator
{
    public TicketPriority[,] priorityMatrix = new TicketPriority[3, 3]
    {
        // Impact:    Low             Medium          High
        { TicketPriority.Low,     TicketPriority.Low,     TicketPriority.Medium },     // Urgency.Low
        { TicketPriority.Low,     TicketPriority.Medium,  TicketPriority.High },       // Urgency.Medium
        { TicketPriority.Medium,  TicketPriority.High,    TicketPriority.Critical }    // Urgency.High
    };

    public TicketPriority CalculatePriority(TicketUrgency urgency, TicketImpact impact)
    {
        return priorityMatrix[(int)urgency, (int)impact];
    }
}