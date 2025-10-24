using System;

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
    TimeSpan? EstimatedStartTime,
    TimeSpan? EstimatedEndTime,
    TimeSpan? EstimatedDuration,
    bool IsDiscussionCompleted,
    bool IsVoteCompleted,
    int? MotionId,
    IEnumerable<AgendaItemValidationDto> Validations,
    IEnumerable<AgendaItemDto> SubItems,
    DiscussionDto? Discussion,
    VotingDto? Voting,
    ElectionDto? Election);

public sealed record AgendaItemTypeDto(int Id, string Name, string? Description);

public sealed record ElectionCandidateDto(string Id, string Name, string? AttendeeId, string? Statement);

public sealed record AgendaItemValidationDto(string Code, string Message, bool IsBlocking);