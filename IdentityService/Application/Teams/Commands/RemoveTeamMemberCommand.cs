using MediatR;

namespace YourBrand.IdentityService.Application.Teams.Commands;

public record RemoveTeamMemberCommand(string TeamId, string UserId) : IRequest
{
} 