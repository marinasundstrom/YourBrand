using YourBrand.Meetings.Features.Organizations;
using YourBrand.Meetings.Features.Users;

namespace YourBrand.Meetings.Features.Procedure.Voting;

public static partial class Mappings
{
    public static VotingDto ToDto(this Domain.Entities.Voting voting) => new(voting.Id, voting.State, voting.HasPassed);
}