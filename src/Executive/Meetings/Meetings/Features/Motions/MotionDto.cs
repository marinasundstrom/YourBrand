namespace YourBrand.Meetings.Features.Motions;

public sealed record MotionDto(int Id, string Title, MotionStatus Status, string Text, IEnumerable<MotionItemDto> Items);
public sealed record MotionItemDto(string Id, string Text);