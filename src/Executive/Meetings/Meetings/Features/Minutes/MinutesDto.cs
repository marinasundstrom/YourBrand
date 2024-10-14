namespace YourBrand.Meetings.Features.Minutes;

public sealed record MinutesDto(int Id, int MeetingId, MinutesState State, IEnumerable<MinutesItemDto> Items);
public sealed record MinutesItemDto(string Id, int Order, MinutesItemType Type, int AgendaId, string AgendaItemId, string Title, MinutesItemState State, string Description, int? MotionId);