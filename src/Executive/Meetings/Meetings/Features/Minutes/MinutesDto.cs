namespace YourBrand.Meetings.Features.Minutes;

public sealed record MinutesDto(int Id, int MeetingId, MinutesState State, IEnumerable<MinutesItemDto> Items);
public sealed record MinutesItemDto(string Id, int Order, AgendaItemType Type, int AgendaId, string AgendaItemId, string Title, MinutesItemState State, string Description, int? MotionId);