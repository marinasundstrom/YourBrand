using Microsoft.EntityFrameworkCore;

using YourBrand.Ticketing.Application.Features.Tickets.Dtos;
using YourBrand.Ticketing.Domain.ValueObjects;

namespace YourBrand.Ticketing.Application;

public interface IDtoComposer
{
    Task<TicketDto> ComposeTicketDto(Ticket ticket, CancellationToken cancellationToken = default);
    Task<IEnumerable<TicketDto>> ComposeTicketDtos(Ticket[] tickets, CancellationToken cancellationToken = default);

    Task<TicketCommentDto> ComposeTicketCommentDto(Ticket ticket, TicketComment ticketComment, CancellationToken cancellationToken = default);
    Task<IEnumerable<TicketCommentDto>> ComposeTicketCommentDtos(Ticket ticket, TicketComment[] ticketComments, CancellationToken cancellationToken = default);

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
        HashSet<ProjectId> projectIds = new();

        projectIds.Add(ticket.ProjectId);

        ExtractTicketParticipantIds(ticket, participantIds);

        var replyTickets = await context.Tickets
            .InOrganization(ticket.OrganizationId)
            .IgnoreQueryFilters()
            .Where(x => ticketIds.Any(z => x.Id == z))
            .ToDictionaryAsync(x => x.Id, x => x, cancellationToken);

        var projects = await context.Projects
            .InOrganization(ticket.OrganizationId)
            .Where(x => projectIds.Any(z => x.Id == z))
            .ToDictionaryAsync(x => x.Id, x => x, cancellationToken);

        /*
        if (replyTickets.Any())
        {
            foreach (var replyTicket in replyTickets.Select(x => x.Value))
            {
                ExtractTicketParticipantIds(replyTicket, participantIds);
            }
        }
        */

        var participants = await context.TicketParticipants
            .InOrganization(ticket.OrganizationId)
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

        return ComposeTicketDtoInternal(ticket, projects, participants, participantIdUsers);
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
        HashSet<ProjectId> projectIds = new();

        if (tickets.Length == 0) return [];

        foreach (var ticket in tickets)
        {
            projectIds.Add(ticket.ProjectId);

            ExtractTicketParticipantIds(ticket, participantIds);
        }

        var projects = await context.Projects
              .InOrganization(tickets.First().OrganizationId)
              .Where(x => projectIds.Any(z => x.Id == z))
              .ToDictionaryAsync(x => x.Id, x => x, cancellationToken);

        var participants = await context.TicketParticipants
            .InOrganization(tickets.First().OrganizationId)
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
            return ComposeTicketDtoInternal(ticket, projects, participants, participantIdUsers);
        });
    }

    private TicketDto ComposeTicketDtoInternal(Ticket ticket, Dictionary<ProjectId, Project> projects, Dictionary<TicketParticipantId, TicketParticipant> participants, Dictionary<TicketParticipantId, User> users)
    {
        projects.TryGetValue(ticket.ProjectId, out var project);

        participants.TryGetValue(ticket.AssigneeId.GetValueOrDefault(), out var assignee);

        participants.TryGetValue(ticket.CreatedById.GetValueOrDefault(), out var createdBy);

        participants.TryGetValue(ticket.LastModifiedById.GetValueOrDefault(), out var editedBy);

        //participants.TryGetValue(ticket.DeletedById.GetValueOrDefault(), out var deletedBy);

        //var reactions = ticket.Reactions.Select(x => dtoFactory.CreateReactionDto(x, participants[x.AddedById], users));

        return dtoFactory.CreateTicketDto(ticket, project!, assignee, createdBy!, editedBy, null!, users);
    }

    public async Task<IEnumerable<TicketEventDto>> ComposeTicketEventDtos(Ticket ticket, TicketEvent[] ticketEvents, CancellationToken cancellationToken = default)
    {
        HashSet<TicketParticipantId> participantIds = new();
        HashSet<TicketId> ticketIds = new();
        HashSet<ProjectId> projectIds = new();


        ExtractTicketParticipantIds(ticket, participantIds);

        foreach (var ev in ticketEvents)
        {
            participantIds.Add(ev.ParticipantId);

            var @event = System.Text.Json.JsonSerializer.Deserialize<TicketDomainEvent>(ev.Data);

            if (@event is TicketAssigneeUpdated e2)
            {
                Console.WriteLine("Foo: " + e2);

                if (e2.OldAssignedParticipantId is not null)
                {
                    participantIds.Add(e2.OldAssignedParticipantId.GetValueOrDefault());
                }

                if (e2.NewAssignedParticipantId is not null)
                {
                    participantIds.Add(e2.NewAssignedParticipantId.GetValueOrDefault());
                }
            }

            if (@event is TicketProjectUpdated e3)
            {
                Console.WriteLine("Foo: " + e3);

                if (e3.OldProjectId is not null)
                {
                    projectIds.Add(e3.OldProjectId);
                }

                if (e3.NewProjectId is not null)
                {
                    projectIds.Add(e3.NewProjectId);
                }
            }
        }

        var participants = await context.TicketParticipants
            .InOrganization(ticket.OrganizationId)
            .Where(x => x.TicketId == ticket.Id)
            .Where(x => participantIds.Any(z => x.Id == z))
            .ToDictionaryAsync(x => x.Id, x => x, cancellationToken);

        var userIds = participants.Select(x => x.Value.UserId).ToList();

        var projects = await context.Projects
            .InOrganization(ticket.OrganizationId)
            .Where(x => projectIds.Any(z => x.Id == z))
            .ToDictionaryAsync(x => x.Id, x => x, cancellationToken);

        var users = await context.Users
            .Where(x => userIds.Any(z => x.Id == z))
            .ToDictionaryAsync(x => x.Id, x => x, cancellationToken);

        var participantIdUsers = participants
            .Select(x => (participantId: x.Key, user: users.FirstOrDefault(x2 => x2.Key == x.Value.UserId).Value))
            .ToDictionary(x => x.participantId, x => x.user);

        return ticketEvents.Select(ticketEvent =>
        {
            return ComposeTicketEventDtoInternal(ticket, ticketEvent, projects, participants, participantIdUsers);
        });
    }

    private TicketEventDto ComposeTicketEventDtoInternal(Ticket ticket, TicketEvent ev, Dictionary<ProjectId, Project> projects, Dictionary<TicketParticipantId, TicketParticipant> participants, Dictionary<TicketParticipantId, User> users)
    {
        var @event = System.Text.Json.JsonSerializer.Deserialize<TicketDomainEvent>(ev.Data);

        participants.TryGetValue(ev.ParticipantId, out var participant);

        if (@event is TicketAssigneeUpdated e2)
        {
            participants.TryGetValue(e2.NewAssignedParticipantId.GetValueOrDefault(), out var newAssignedParticipant);

            participants.TryGetValue(e2.OldAssignedParticipantId.GetValueOrDefault(), out var oldAssignedParticipant);

            return new TicketAssigneeUpdatedDto(ev.OccurredAt, ev.TenantId, ev.OrganizationId, e2.TicketId, newAssignedParticipant is null ? null : dtoFactory.CreateParticipantDto(newAssignedParticipant!, users), oldAssignedParticipant is null ? null : dtoFactory.CreateParticipantDto(oldAssignedParticipant!, users), dtoFactory.CreateParticipantDto(participant!, users));
        }

        if (@event is TicketProjectUpdated e4)
        {
            projects.TryGetValue(e4.NewProjectId, out var newProject);

            projects.TryGetValue(e4.OldProjectId, out var oldProject);

            return new TicketProjectUpdatedDto(ev.OccurredAt, ev.TenantId, ev.OrganizationId, e4.TicketId, newProject is null ? null : dtoFactory.CreateProjectDto(newProject!), oldProject is null ? null : dtoFactory.CreateProjectDto(oldProject!), dtoFactory.CreateParticipantDto(participant!, users));
        }

        return @event switch
        {
            TicketCreated e => new TicketCreatedDto(ev.OccurredAt, ev.TenantId, ev.OrganizationId, e.TicketId, dtoFactory.CreateParticipantDto(participant!, users)),
            TicketDescriptionUpdated e => new TicketDescriptionUpdatedDto(ev.OccurredAt, ev.TenantId, ev.OrganizationId, e.TicketId, e.NewDescription, e.OldDescription, dtoFactory.CreateParticipantDto(participant!, users)),
            TicketEstimatedHoursUpdated e => new TicketEstimatedHoursUpdatedDto(ev.OccurredAt, ev.TenantId, ev.OrganizationId, e.TicketId, e.NewHours, e.OldHours, dtoFactory.CreateParticipantDto(participant!, users)),
            TicketRemainingHoursUpdated e => new TicketRemainingHoursUpdatedDto(ev.OccurredAt, ev.TenantId, e.OrganizationId, e.TicketId, e.NewHours, e.OldHours, dtoFactory.CreateParticipantDto(participant!, users)),
            TicketStatusUpdated e => new TicketStatusUpdatedDto(ev.OccurredAt, ev.TenantId, ev.OrganizationId, e.TicketId, new TicketStatusDto(e.NewStatus.Id, e.NewStatus.Name), new TicketStatusDto(e.OldStatus.Id, e.OldStatus.Name), dtoFactory.CreateParticipantDto(participant!, users)),
            TicketSubjectUpdated e => new TicketSubjectUpdatedDto(ev.OccurredAt, ev.TenantId, ev.OrganizationId, e.TicketId, e.NewSubject, e.OldSubject, dtoFactory.CreateParticipantDto(participant!, users)),
            TicketPriorityUpdated e => new TicketPriorityUpdatedDto(ev.OccurredAt, ev.TenantId, ev.OrganizationId, e.TicketId, (TicketPriorityDto?)e.NewPriority, (TicketPriorityDto?)e.OldPriority, dtoFactory.CreateParticipantDto(participant!, users)),
            TicketImpactUpdated e => new TicketImpactUpdatedDto(ev.OccurredAt, ev.TenantId, ev.OrganizationId, e.TicketId, (TicketImpactDto?)e.NewImpact, (TicketImpactDto?)e.OldImpact, dtoFactory.CreateParticipantDto(participant!, users)),
            TicketUrgencyUpdated e => new TicketUrgencyUpdatedDto(ev.OccurredAt, ev.TenantId, ev.OrganizationId, e.TicketId, (TicketUrgencyDto?)e.NewUrgency, (TicketUrgencyDto?)e.OldUrgency, dtoFactory.CreateParticipantDto(participant!, users)),
            TicketCommentAdded e => new TicketCommentAddedDto(ev.OccurredAt, ev.TenantId, ev.OrganizationId, e.TicketId, e.CommentId, dtoFactory.CreateParticipantDto(participant!, users)),

            _ => throw new Exception()
        };
    }

    public async Task<TicketCommentDto> ComposeTicketCommentDto(Ticket ticket, TicketComment ticketComment, CancellationToken cancellationToken = default)
    {
        HashSet<TicketParticipantId> participantIds = new();
        HashSet<TicketId> ticketIds = new();

        ExtractTicketParticipantIds(ticket, participantIds);

        participantIds.Add(ticketComment.CreatedById.GetValueOrDefault());

        if (ticketComment.LastModifiedById is not null)
        {
            participantIds.Add(ticketComment.LastModifiedById.GetValueOrDefault());
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

        return ComposeTicketCommentDtoInternal(ticket, ticketComment, participants, participantIdUsers);
    }

    public async Task<IEnumerable<TicketCommentDto>> ComposeTicketCommentDtos(Ticket ticket, TicketComment[] ticketComments, CancellationToken cancellationToken = default)
    {
        HashSet<TicketParticipantId> participantIds = new();
        HashSet<TicketId> ticketIds = new();

        ExtractTicketParticipantIds(ticket, participantIds);

        foreach (var comment in ticketComments)
        {
            participantIds.Add(comment.CreatedById.GetValueOrDefault());

            if (comment.LastModifiedById is not null)
            {
                participantIds.Add(comment.LastModifiedById.GetValueOrDefault());
            }

            /*
            if (comment.NewAssignedParticipantId is not null)
            {
                participantIds.Add(e2.NewAssignedParticipantId.GetValueOrDefault());
            }
            */
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

        return ticketComments.Select(ticketComment =>
        {
            return ComposeTicketCommentDtoInternal(ticket, ticketComment, participants, participantIdUsers);
        });
    }

    private TicketCommentDto ComposeTicketCommentDtoInternal(Ticket ticket, TicketComment ticketComment, Dictionary<TicketParticipantId, TicketParticipant> participants, Dictionary<TicketParticipantId, User> users)
    {
        participants.TryGetValue(ticketComment.CreatedById.GetValueOrDefault(), out var createdBy);

        participants.TryGetValue(ticketComment.LastModifiedById.GetValueOrDefault(), out var editedBy);

        //participants.TryGetValue(ticket.DeletedById.GetValueOrDefault(), out var deletedBy);

        return dtoFactory.CreateTicketCommentDto(ticketComment, createdBy!, editedBy!, null!, users);
    }
}