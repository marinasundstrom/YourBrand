using MediatR;

using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

using YourBrand.Identity;
using YourBrand.Meetings.Features.Agendas;

namespace YourBrand.Meetings.Features.Procedure.Chairman;

public sealed record EndDiscussions(string OrganizationId, int Id) : IRequest<Result>
{
    public sealed class Handler(IApplicationDbContext context, IUserContext userContext, IHubContext<MeetingsProcedureHub, IMeetingsProcedureHubClient> hubContext) : IRequestHandler<EndDiscussions, Result>
    {
        public async Task<Result> Handle(EndDiscussions request, CancellationToken cancellationToken)
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

            var attendee = meeting.GetAttendeeByUserId(userContext.UserId);

            if (attendee is null)
            {
                return Errors.Meetings.YouAreNotAttendeeOfMeeting;
            }

            var agendaItem = meeting.GetCurrentAgendaItem();

            if (agendaItem is null)
            {
                return Errors.Meetings.NoActiveAgendaItem;
            }

            if (attendee.Role != AttendeeRole.Chairperson)
            {
                return Errors.Meetings.OnlyChairpersonCanEndDiscussion;
            }

            agendaItem.EndDiscussion();

            context.Meetings.Update(meeting);

            await context.SaveChangesAsync(cancellationToken);

            await hubContext.Clients
                .Group($"meeting-{meeting.Id}")
               .OnAgendaItemStateChanged(agendaItem.Id, (Dtos.AgendaItemState)agendaItem.State);

            return Result.Success;
        }
    }
}