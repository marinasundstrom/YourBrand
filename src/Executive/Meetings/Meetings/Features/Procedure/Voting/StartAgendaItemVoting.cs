using MediatR;

using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

using YourBrand.Identity;
using YourBrand.Meetings.Features.Agendas;
using YourBrand.Meetings.Features.Procedure.Command;

namespace YourBrand.Meetings.Features.Procedure.Voting;

public sealed record StartAgendaItemVoting(string OrganizationId, int Id) : IRequest<Result>
{
    public sealed class Handler(IApplicationDbContext context, IUserContext userContext, IHubContext<MeetingsProcedureHub, IMeetingsProcedureHubClient> hubContext) : IRequestHandler<StartAgendaItemVoting, Result>
    {
        public async Task<Result> Handle(StartAgendaItemVoting request, CancellationToken cancellationToken)
        {
            var meeting = await context.Meetings
                .InOrganization(request.OrganizationId)
                .Include(x => x.Agenda)
                .ThenInclude(x => x.Items.OrderBy(x => x.Order ))
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
                return Errors.Meetings.OnlyChairpersonCanStartVotingSession;
            }
            
            agendaItem.StartVoting();

            context.Meetings.Update(meeting);

            await context.SaveChangesAsync(cancellationToken);

            await hubContext.Clients
                .Group($"meeting-{meeting.Id}")
                .OnAgendaItemStatusChanged(agendaItem.Id);

            return Result.Success;
        }
    }
}

