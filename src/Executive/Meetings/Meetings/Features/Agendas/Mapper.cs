using YourBrand.Meetings.Features.Procedure.Discussions;
using YourBrand.Meetings.Features.Procedure.Voting;
using YourBrand.Meetings.Features.Procedure.Elections;

namespace YourBrand.Meetings.Features.Agendas;

public static partial class Mappings
{
    public static AgendaDto ToDto(this Agenda agenda) => new(agenda.Id, agenda.State, agenda.Items.Select(x => x.ToDto()));
    public static AgendaItemDto ToDto(this AgendaItem item) => new(
        item.Id, item.ParentId, item.Order, item.Type.ToDto(), item.Title!, item.State, item.Description,
        item.IsMandatory,
        item.DiscussionActions,
        item.VoteActions,
        item.IsDiscussionCompleted,
        item.IsVoteCompleted,
        item.MotionId,
        item.SubItems.Select(x => x.ToDto()),
        item.Discussion?.ToDto(),
        item.Voting?.ToDto(),
        item.Election?.ToDto());


    public static AgendaItemTypeDto ToDto(this AgendaItemType type) =>
        new(type.Id, type.Name, type.Description);

    public static ElectionCandidateDto ToDto(this ElectionCandidate candidate) =>
        new(candidate.Id, candidate.Name, candidate.AttendeeId, candidate.Statement);
}