using Microsoft.EntityFrameworkCore;

using YourBrand.Ticketing.Domain.ValueObjects;
using YourBrand.Ticketing.Application.Features.Tickets.Dtos;

namespace YourBrand.Ticketing.Application;

public interface IDtoComposer
{
    Task<TicketDto> ComposeTicketDto(Ticket ticket, CancellationToken cancellationToken = default);
    Task<IEnumerable<TicketDto>> ComposeTicketDtos(Ticket[] tickets, CancellationToken cancellationToken = default);
    Task<IEnumerable<TicketEventDto>> ComposeTicketEventDtos(Ticket ticket, TicketEvent[] ticketEvents, CancellationToken cancellationToken = default);
}

public sealed class DtoComposer : IDtoComposer
{
    private readonly IApplicationDbContext context;
    private readonly IDtoFactory dtoFactory;

    public DtoComposer(IApplicationDbContext context, IDtoFactory dtoFactory)
    {
        this.context = context;
        this.dtoFactory = dtoFactory;
    }

    public async Task<TicketDto> ComposeTicketDto(Ticket ticket, CancellationToken cancellationToken = default)
    {
        HashSet<TicketParticipantId> participantIds = new();
        HashSet<TicketId> ticketIds = new();

        ExtractTicketParticipantIds(ticket, participantIds);

        var replyTickets = await context.Tickets
            .IgnoreQueryFilters()
            .Where(x => ticketIds.Any(z => x.Id == z))
            .ToDictionaryAsync(x => x.Id, x => x, cancellationToken);

        if (replyTickets.Any())
        {
            foreach (var replyTicket in replyTickets.Select(x => x.Value))
            {
                ExtractTicketParticipantIds(replyTicket, participantIds);
            }
        }

        var participants = await context.TicketParticipants
            .Where(x => x.TicketId == ticket.Id)
            .Where(x => participantIds.Any(z => x.Id == z))
            .ToDictionaryAsync(x => x.Id, x => x, cancellationToken);

        var userIds = participants.Select(x => x.Value.UserId).ToList();

        var users = await context.Users
            .Where(x => userIds.Any(z => x.Id == z))
            .ToDictionaryAsync(x => x.Id, x => x, cancellationToken);

        var participantIdUsers = participants
            .Select(x => (participantId: x.Key, user: users.FirstOrDefault(x2 => x2.Key == x.Value.UserId).Value))
            .ToDictionary(x => x.participantId, x => x.user);

        return ComposeTicketDtoInternal(ticket, participants, participantIdUsers);
    }

    private static void ExtractTicketParticipantIds(Ticket ticket, HashSet<TicketParticipantId> participantIds)
    {
        if (ticket.CreatedById is not null)
        {
            participantIds.Add(ticket.CreatedById.GetValueOrDefault());
        }

        if (ticket.LastModifiedById is not null)
        {
            participantIds.Add(ticket.LastModifiedById.GetValueOrDefault());
        }

        if (ticket.AssigneeId is not null)
        {
            participantIds.Add(ticket.AssigneeId.GetValueOrDefault());
        }
    }

    public async Task<IEnumerable<TicketDto>> ComposeTicketDtos(Ticket[] tickets, CancellationToken cancellationToken = default)
    {
        HashSet<TicketParticipantId> participantIds = new();
        HashSet<TicketId> ticketIds = new();

        foreach (var ticket in tickets)
        {
            ExtractTicketParticipantIds(ticket, participantIds);
        }

        var participants = await context.TicketParticipants
            //.Where(x => x.ChannelId == ticket.ChannelId)
            .Where(x => participantIds.Any(z => x.Id == z))
            .ToDictionaryAsync(x => x.Id, x => x, cancellationToken);

        var userIds = participants.Select(x => x.Value.UserId).ToList();

        var users = await context.Users
            .Where(x => userIds.Any(z => x.Id == z))
            .ToDictionaryAsync(x => x.Id, x => x, cancellationToken);

        var participantIdUsers = participants
            .Select(x => (participantId: x.Key, user: users.FirstOrDefault(x2 => x2.Key == x.Value.UserId).Value))
            .ToDictionary(x => x.participantId, x => x.user);

        return tickets.Select(ticket =>
        {
            return ComposeTicketDtoInternal(ticket, participants, participantIdUsers);
        });
    }

    private TicketDto ComposeTicketDtoInternal(Ticket ticket, Dictionary<TicketParticipantId, TicketParticipant> participants, Dictionary<TicketParticipantId, User> users)
    {
        participants.TryGetValue(ticket.AssigneeId.GetValueOrDefault(), out var assignee);

        participants.TryGetValue(ticket.CreatedById.GetValueOrDefault(), out var createdBy);

        participants.TryGetValue(ticket.LastModifiedById.GetValueOrDefault(), out var editedBy);

        //participants.TryGetValue(ticket.DeletedById.GetValueOrDefault(), out var deletedBy);

        //var reactions = ticket.Reactions.Select(x => dtoFactory.CreateReactionDto(x, participants[x.AddedById], users));

        return dtoFactory.CreateTicketDto(ticket, assignee, createdBy!, editedBy, null!, users);
    }

    public async Task<IEnumerable<TicketEventDto>> ComposeTicketEventDtos(Ticket ticket, TicketEvent[] ticketEvents, CancellationToken cancellationToken = default)
    {
        HashSet<TicketParticipantId> participantIds = new();
        HashSet<TicketId> ticketIds = new();

        ExtractTicketParticipantIds(ticket, participantIds);

        foreach(var ev in ticketEvents) 
        {
            participantIds.Add(ev.ParticipantId);

            var @event = System.Text.Json.JsonSerializer.Deserialize<TicketDomainEvent>(ev.Data);

            if (@event is TicketAssigneeUpdated e2)
            {
                Console.WriteLine("Foo: " + e2);

                if(e2.OldAssignedParticipantId is not null) 
                {
                    participantIds.Add(e2.OldAssignedParticipantId.GetValueOrDefault());
                }

                if (e2.NewAssignedParticipantId is not null)
                {
                    participantIds.Add(e2.NewAssignedParticipantId.GetValueOrDefault());
                }
            }
        }

        var participants = await context.TicketParticipants
            .Where(x => x.TicketId == ticket.Id)
            .Where(x => participantIds.Any(z => x.Id == z))
            .ToDictionaryAsync(x => x.Id, x => x, cancellationToken);

        var userIds = participants.Select(x => x.Value.UserId).ToList();

        var users = await context.Users
            .Where(x => userIds.Any(z => x.Id == z))
            .ToDictionaryAsync(x => x.Id, x => x, cancellationToken);

        var participantIdUsers = participants
            .Select(x => (participantId: x.Key, user: users.FirstOrDefault(x2 => x2.Key == x.Value.UserId).Value))
            .ToDictionary(x => x.participantId, x => x.user);

        return ticketEvents.Select(ticketEvent =>
        {
            return ComposeTicketEventDtoInternal(ticket, ticketEvent, participants, participantIdUsers);
        });
    }

    private TicketEventDto ComposeTicketEventDtoInternal(Ticket ticket, TicketEvent ev, Dictionary<TicketParticipantId, TicketParticipant> participants, Dictionary<TicketParticipantId, User> users) 
    {
        var @event = System.Text.Json.JsonSerializer.Deserialize<TicketDomainEvent>(ev.Data);

        participants.TryGetValue(ev.ParticipantId, out var participant);

        if(@event is TicketAssigneeUpdated e2) 
        {
            participants.TryGetValue(e2.NewAssignedParticipantId.GetValueOrDefault(), out var newAssignedParticipant);

            participants.TryGetValue(e2.OldAssignedParticipantId.GetValueOrDefault(), out var oldAssignedParticipant);

            return new TicketAssigneeUpdatedDto(ev.OccurredAt, ev.TenantId, ev.OrganizationId, e2.TicketId, newAssignedParticipant is null ? null : dtoFactory.CreateParticipantDto(newAssignedParticipant!, users), oldAssignedParticipant is null ? null : dtoFactory.CreateParticipantDto(oldAssignedParticipant!, users), dtoFactory.CreateParticipantDto(participant!, users));
        }

        return @event switch
        {
            TicketCreated e => new TicketCreatedDto(ev.OccurredAt, ev.TenantId, ev.OrganizationId, e.TicketId, dtoFactory.CreateParticipantDto(participant!, users)),
            TicketDescriptionUpdated e => new TicketDescriptionUpdatedDto(ev.OccurredAt, ev.TenantId, ev.OrganizationId, e.TicketId, e.NewDescription, e.OldDescription, dtoFactory.CreateParticipantDto(participant!, users)),
            TicketEstimatedHoursUpdated e => new TicketEstimatedHoursUpdatedDto(ev.OccurredAt, ev.TenantId, ev.OrganizationId, e.TicketId, e.NewHours, e.OldHours, dtoFactory.CreateParticipantDto(participant!, users)),
            TicketRemainingHoursUpdated e => new TicketRemainingHoursUpdatedDto(ev.OccurredAt, ev.TenantId, e.OrganizationId, e.TicketId, e.NewHours, e.OldHours, dtoFactory.CreateParticipantDto(participant!, users)),
            TicketStatusUpdated e => new TicketStatusUpdatedDto(ev.OccurredAt, ev.TenantId, ev.OrganizationId, e.TicketId, new TicketStatusDto(e.NewStatus.Id, e.NewStatus.Name), new TicketStatusDto(e.OldStatus.Id, e.OldStatus.Name), dtoFactory.CreateParticipantDto(participant!, users)),
            TicketSubjectUpdated e => new TicketSubjectUpdatedDto(ev.OccurredAt, ev.TenantId, ev.OrganizationId, e.TicketId, e.NewSubject, e.OldSubject, dtoFactory.CreateParticipantDto(participant!, users)),
            _ => throw new Exception()
        };
    }
}