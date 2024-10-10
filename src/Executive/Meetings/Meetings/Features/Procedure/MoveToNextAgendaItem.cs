using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Identity;
using YourBrand.Meetings.Features.Agendas;

namespace YourBrand.Meetings.Features.Procedure.Command;

public sealed record MoveToNextAgendaItem(string OrganizationId, int Id) : IRequest<Result<AgendaItemDto>>
{
    public sealed class Handler(IApplicationDbContext context, IUserContext userContext) : IRequestHandler<MoveToNextAgendaItem, Result<AgendaItemDto>>
    {
        public async Task<Result<AgendaItemDto>> Handle(MoveToNextAgendaItem request, CancellationToken cancellationToken)
        {
            var meeting = await context.Meetings
                .InOrganization(request.OrganizationId)
                .Include(x => x.Agenda)
                .ThenInclude(x => x.Items.OrderBy(x => x.Order))
                .FirstOrDefaultAsync(x => x.Id == request.Id);

            if (meeting is null)
            {
                return Errors.Meetings.MeetingNotFound;
            }

            var participant = meeting.Participants.FirstOrDefault(x => x.UserId == userContext.UserId);

            if(participant is null)
                throw new UnauthorizedAccessException("You are not a participant of this meeting.");

            if (participant.Role != ParticipantRole.Chairperson)
                throw new UnauthorizedAccessException("Only the Chairperson can start the meeting.");

            meeting.MoveToNextAgendaItem();

            context.Meetings.Update(meeting);

            await context.SaveChangesAsync(cancellationToken);

            var nextItem = meeting.Agenda!.Items.ElementAt(meeting.CurrentAgendaItemIndex.GetValueOrDefault());

            return nextItem.ToDto();
        }
    }
}

