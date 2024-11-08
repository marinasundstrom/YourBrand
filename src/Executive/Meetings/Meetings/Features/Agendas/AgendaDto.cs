namespace YourBrand.Meetings.Features.Agendas;

public sealed record AgendaDto(int Id, AgendaState State, IEnumerable<AgendaItemDto> Items);

public sealed record AgendaItemDto(
    string Id, 
    string? ParentId, 
    int Order, 
    AgendaItemTypeDto Type, 
    string Title, 
    AgendaItemState State, 
    string Description,
    bool IsMandatory,
    DiscussionActions DiscussionActions,
    VoteActions VoteActions,
    bool IsDiscussionCompleted,
    bool IsVoteCompleted,
    int? MotionId,
    IEnumerable<AgendaItemDto> SubItems);

public sealed record AgendaItemTypeDto(int Id, string Name, string? Description);