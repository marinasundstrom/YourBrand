namespace YourBrand.Meetings.Features.Motions;

public sealed record MotionDto(int Id, string Title, MotionStatus Status, string Text, IEnumerable<MotionOperativeClauseDto> OperativeClauses);
public sealed record MotionOperativeClauseDto(string Id, int Order , OperativeAction Action, string Text);