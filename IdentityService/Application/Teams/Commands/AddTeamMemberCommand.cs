using MediatR;

namespace YourBrand.IdentityService.Application.Teams.Commands;

public record AddTeamMemberCommand(string TeamId, string UserId) : IRequest
{
} 
