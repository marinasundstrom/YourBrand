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
        if (!Enum.IsDefined(typeof(TicketUrgency), urgency) ||
            !Enum.IsDefined(typeof(TicketImpact), impact))
        {
            throw new ArgumentOutOfRangeException("Invalid urgency or impact value.");
        }

        return priorityMatrix[(int)urgency, (int)impact];
    }
}