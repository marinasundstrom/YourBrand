namespace YourBrand.Meetings.Features.Procedure.Voting;

public sealed record VotingDto(string Id, VotingState State, bool HasPassed);
