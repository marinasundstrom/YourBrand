using YourBrand.Meetings.Features.Organizations;
using YourBrand.Meetings.Features.Users;

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
        item.Candidates.Select(x => x.ToDto()));


    public static AgendaItemTypeDto ToDto(this AgendaItemType type) =>
        new(type.Id, type.Name, type.Description);

    public static ElectionCandidateDto ToDto(this ElectionCandidate candidate) =>
        new(candidate.Id, $"Attendee {candidate.NomineeId}", candidate.NomineeId, candidate.Statement);
}