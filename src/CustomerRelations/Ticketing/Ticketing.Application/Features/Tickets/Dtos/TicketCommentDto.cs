namespace YourBrand.Ticketing.Application.Features.Tickets.Dtos;

using YourBrand.Ticketing.Application.Features.Users;

public sealed record TicketCommentDto(int Id, string Text, DateTimeOffset Created, TicketParticipantDto? CreatedBy, DateTimeOffset? LastModified, TicketParticipantDto? LastModifiedBy);