using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;

using YourBrand.Ticketing.Domain.ValueObjects;
using YourBrand.Ticketing.Application.Features.Tickets.Dtos;
using YourBrand.Ticketing.Application.Features.Users;

namespace YourBrand.Ticketing.Application;

public interface IDtoFactory
{
    TicketDto CreateTicketDto(Ticket ticket, TicketParticipant? assignee, TicketParticipant createdBy, TicketParticipant? editedBy, TicketParticipant? deletedBy, Dictionary<TicketParticipantId, User> users);
    TicketParticipantDto CreateParticipantDto(TicketParticipant participant, Dictionary<TicketParticipantId, User> users);
    UserDto CreateUserDto(User user);
}

public sealed class DtoFactory : IDtoFactory
{
    public TicketDto CreateTicketDto(Ticket ticket, TicketParticipant? assignee, TicketParticipant createdBy, TicketParticipant? editedBy, TicketParticipant? deletedBy, Dictionary<TicketParticipantId, User> users)
    {
        return new TicketDto(
            ticket.Id,
            ticket.Subject,
            ticket.Text,
            ticket.Status.ToDto()!,
            ticket.AssigneeId is null ? null : CreateParticipantDto(assignee!, users),
            ticket.LastMessage,
            ticket.Text,
            ticket.Type!.ToDto(),
            ticket.Priority.ToDto(),
            ticket.Urgency.ToDto(),
            ticket.Impact.ToDto(),
            ticket.EstimatedHours,
            ticket.RemainingHours,
            ticket.Tags.Select(x => x.Tag).Select(x => x.ToDto()),
            ticket.Attachments.Select(x => x.ToDto()),
            ticket.Created,
            CreateParticipantDto(createdBy, users),
            ticket.LastModified,
            ticket.LastModifiedById is null ? null : CreateParticipantDto(editedBy!, users));
    }

    public UserDto CreateUserDto(User user)
    {
        return new UserDto(user!.Id.ToString(), user.Name);
    }

    public TicketParticipantDto CreateParticipantDto(TicketParticipant participant, Dictionary<TicketParticipantId, User> users)
    {
        return new TicketParticipantDto(
            participant!.Id,
            /* participant.DisplayName ?? */ users[participant.Id].Name,
            participant.UserId);
    }
}