using System.Linq;

namespace YourBrand.Meetings.Features.Procedure.Voting;

public static class Mapper
{
    public static VotingDto ToDto(this Domain.Entities.Voting voting)
    {
        VoteBreakdownDto? breakdown = null;

        if (voting.State == Domain.Entities.VotingState.ResultReady)
        {
            var counts = voting.Votes
                .Where(v => v.Option.HasValue)
                .GroupBy(v => v.Option!.Value)
                .ToDictionary(g => g.Key, g => g.Count());

            breakdown = new VoteBreakdownDto(
                counts.GetValueOrDefault(Domain.Entities.VoteOption.For),
                counts.GetValueOrDefault(Domain.Entities.VoteOption.Against),
                counts.GetValueOrDefault(Domain.Entities.VoteOption.Abstain));
        }

        return new VotingDto(voting.Id, voting.State, voting.HasPassed, breakdown);
    }
}
