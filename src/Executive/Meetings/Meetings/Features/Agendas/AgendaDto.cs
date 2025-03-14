using YourBrand.Meetings.Features.Procedure.Discussions;
using YourBrand.Meetings.Features.Procedure.Voting;
using YourBrand.Meetings.Features.Procedure.Elections;

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
    IEnumerable<AgendaItemDto> SubItems,
    DiscussionDto? Discussion,
    VotingDto? Voting,
    ElectionDto? Election);

public sealed record AgendaItemTypeDto(int Id, string Name, string? Description);

public sealed record ElectionCandidateDto(string Id, string Name, string? AttendeeId, string? Statement);