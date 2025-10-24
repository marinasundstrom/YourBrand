using System;
using System.Collections.Generic;
using System.Linq;

using YourBrand.Meetings.Features.Procedure.Discussions;
using YourBrand.Meetings.Features.Procedure.Elections;
using YourBrand.Meetings.Features.Procedure.Voting;

namespace YourBrand.Meetings.Features.Agendas;

public static partial class Mappings
{
    public static AgendaDto ToDto(this Agenda agenda, IReadOnlyDictionary<string, IReadOnlyCollection<AgendaItemValidation>>? validations = null)
        => new(agenda.Id, agenda.State, agenda.Items.OrderBy(x => x.Order).Select(x => x.ToDto(validations)));

    public static AgendaItemDto ToDto(this AgendaItem item, IReadOnlyDictionary<string, IReadOnlyCollection<AgendaItemValidation>>? validations = null)
    {
        var itemValidations = validations is not null && validations.TryGetValue(item.Id, out var requirements)
            ? requirements.Select(x => x.ToDto()).ToList()
            : Array.Empty<AgendaItemValidationDto>();

        return new(
            item.Id, item.ParentId, item.Order, item.Type.ToDto(), item.Title!, item.State, item.Description,
            item.IsMandatory,
            item.DiscussionActions,
            item.VoteActions,
            item.EstimatedStartTime,
            item.EstimatedEndTime,
            item.EstimatedDuration,
            item.IsDiscussionCompleted,
            item.IsVoteCompleted,
            item.MotionId,
            itemValidations,
            item.SubItems.OrderBy(x => x.Order).Select(x => x.ToDto(validations)),
            item.Discussion?.ToDto(),
            item.Voting?.ToDto(),
            item.Election?.ToDto());
    }

    public static AgendaItemTypeDto ToDto(this AgendaItemType type) =>
        new(type.Id, type.Name, type.Description);

    public static ElectionCandidateDto ToDto(this ElectionCandidate candidate) =>
        new(candidate.Id, candidate.Name, candidate.AttendeeId, candidate.Statement);

    public static AgendaItemValidationDto ToDto(this AgendaItemValidation validation) =>
        new(validation.Code, validation.Message, validation.IsBlocking);
}
