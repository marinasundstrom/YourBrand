namespace YourBrand.Meetings.Features.Procedure.Voting;

public sealed record VotingSessionDto(string Id, VotingState State, bool HasPassed);
