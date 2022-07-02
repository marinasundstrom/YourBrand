using MediatR;

namespace YourBrand.HumanResources.Application.Teams.Commands;

public record RemoveTeamMemberCommand(string TeamId, string UserId) : IRequest
{
} 