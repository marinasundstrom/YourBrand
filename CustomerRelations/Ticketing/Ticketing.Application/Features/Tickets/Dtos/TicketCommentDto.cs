namespace YourBrand.Ticketing.Application.Features.Tickets.Dtos;

using YourBrand.Ticketing.Application.Features.Users;

public sealed record TicketCommentDto(int Id, string Text, DateTimeOffset Created, UserDto? CreatedBy, DateTimeOffset? LastModified, UserDto? LastModifiedBy);
