using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using YourBrand.Ticketing.Domain.Entities;

namespace YourBrand.Ticketing.Infrastructure.Persistence;

public static class Seed
{
    public static async Task SeedData(ApplicationDbContext context)
    {
        context.TicketStatuses.Add(new TicketStatus() {
            Name = "New"
        });

        context.TicketStatuses.Add(new TicketStatus() {
            Name = "In progress"
        });

        context.TicketStatuses.Add(new TicketStatus() {
            Name = "On hold"
        });

        context.TicketStatuses.Add(new TicketStatus() {
            Name = "Resolved"
        });

        context.TicketStatuses.Add(new TicketStatus() {
            Name = "Closed"
        });

        context.TicketTypes.Add(new TicketType("Ticket"));

        await context.SaveChangesAsync();
    }
}