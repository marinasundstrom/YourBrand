namespace YourBrand.Meetings.Features.Agendas;

public sealed record AgendaDto(int Id, IEnumerable<AgendaItemDto> Items);
public sealed record AgendaItemDto(string Id, int Order, string Title, AgendaItemState State, string Description, int? MotionId);