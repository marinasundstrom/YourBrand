using MediatR;

namespace YourBrand.HumanResources.Application.Teams.Commands;

public record AddTeamMemberCommand(string TeamId, string UserId) : IRequest
{
} 
