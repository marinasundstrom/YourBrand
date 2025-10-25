namespace YourBrand.Meetings.Features.Procedure.Voting;

public sealed record VoteBreakdownDto(int ForVotes, int AgainstVotes, int AbstainVotes);

public sealed record VotingDto(string Id, VotingState State, bool HasPassed, VoteBreakdownDto? Breakdown);
