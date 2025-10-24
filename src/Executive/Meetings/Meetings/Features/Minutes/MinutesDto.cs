using System;

using YourBrand.Meetings.Features.Agendas;

namespace YourBrand.Meetings.Features.Minutes;

public sealed record MinutesDto(int Id, int MeetingId, MinutesState State, IEnumerable<MinutesItemDto> Items)
{
    public IEnumerable<MinutesTaskDto> Tasks { get; init; } = Array.Empty<MinutesTaskDto>();
}

public sealed record MinutesItemDto(string Id, int Order, AgendaItemTypeDto Type, int AgendaId, string AgendaItemId, string Title, MinutesItemState State, string Description, int? MotionId);
public sealed record MinutesTaskDto(
    string Id,
    MinutesTaskType Type,
    MinutesTaskStatus Status,
    string Title,
    string? Description,
    DateTimeOffset AssignedAt,
    DateTimeOffset? DueAt,
    string? AssignedToId,
    string? AssignedToName,
    string? AssignedToEmail,
    DateTimeOffset? CompletedAt);