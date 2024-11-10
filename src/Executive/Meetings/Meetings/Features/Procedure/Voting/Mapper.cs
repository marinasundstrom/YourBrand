using YourBrand.Meetings.Features.Organizations;
using YourBrand.Meetings.Features.Users;

namespace YourBrand.Meetings.Features.Procedure.Voting;

public static partial class Mappings
{
    public static VotingSessionDto ToDto(this Domain.Entities.Voting votingSession) => new(votingSession.Id, votingSession.State, votingSession.HasPassed);
}