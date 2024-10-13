using MediatR;

using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

using YourBrand.Identity;

namespace YourBrand.Meetings.Features.Procedure.Command;

public sealed record PostponeAgendaItem(string OrganizationId, int Id) : IRequest<Result>
{
    public sealed class Handler(IApplicationDbContext context, IUserContext userContext, IHubContext<MeetingsProcedureHub, IMeetingsProcedureHubClient> hubContext) : IRequestHandler<PostponeAgendaItem, Result>
    {
        public async Task<Result> Handle(PostponeAgendaItem request, CancellationToken cancellationToken)
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

            if (participant is null)
            {
                return Errors.Meetings.YouAreNotParticipantOfMeeting;
            }

            var agendaItem = meeting.GetCurrentAgendaItem();

            if (agendaItem is null)
            {
                return Errors.Meetings.NoActiveAgendaItem;
            }

            if (participant.Role != ParticipantRole.Chairperson)
            {
                return Errors.Meetings.OnlyChairpersonCanPostponeAgendaItem;
            }

            agendaItem.Postpone();

            context.Meetings.Update(meeting);

            await context.SaveChangesAsync(cancellationToken);

            await hubContext.Clients
                .Group($"meeting-{meeting.Id}")
                .OnAgendaItemStatusChanged(agendaItem.Id);

            return Result.Success;
        }
    }
}
