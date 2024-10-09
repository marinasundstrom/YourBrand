namespace YourBrand.Meetings.Features.Agendas;

public sealed record AgendaDto(int Id, AgendaState State, IEnumerable<AgendaItemDto> Items);
public sealed record AgendaItemDto(string Id, int Order, AgendaItemType Type, string Title, AgendaItemState State, string Description, int? MotionId);