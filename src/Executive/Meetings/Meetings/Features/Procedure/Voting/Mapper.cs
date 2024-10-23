using YourBrand.Meetings.Features.Organizations;
using YourBrand.Meetings.Features.Users;

namespace YourBrand.Meetings.Features.Procedure.Voting;

public static partial class Mappings
{
    public static VotingSessionDto ToDto(this VotingSession votingSession) => new(votingSession.Id);
}