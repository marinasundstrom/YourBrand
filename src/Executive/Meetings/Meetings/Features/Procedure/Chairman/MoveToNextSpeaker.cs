using MediatR;

using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

using YourBrand.Identity;
using YourBrand.Meetings.Features.Agendas;
using YourBrand.Meetings.Features.Procedure.Command;
using YourBrand.Meetings.Features.Procedure.Discussions;

namespace YourBrand.Meetings.Features.Procedure.Chairman;

public sealed record MoveToNextSpeaker(string OrganizationId, int Id) : IRequest<Result>
{
    public sealed class Handler(IApplicationDbContext context, IUserContext userContext, IHubContext<MeetingsProcedureHub, IMeetingsProcedureHubClient> hubContext) : IRequestHandler<MoveToNextSpeaker, Result>
    {
        public async Task<Result> Handle(MoveToNextSpeaker request, CancellationToken cancellationToken)
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

            if (attendee.Role != AttendeeRole.Chairperson)
            {
                return Errors.Meetings.OnlyChairpersonCanManageSpeakerQueue;
            }

            var agendaItem = meeting.GetCurrentAgendaItem();

            if (agendaItem is null)
            {
                return Errors.Meetings.AgendaItemNotFound;
            }

            if (agendaItem.Discussion is null)
            {
                return Errors.Meetings.NoOngoingDiscussionSession;
            }

            var speakerRequest = agendaItem.Discussion!.MoveToNextSpeaker();

            var speakerRequestId = speakerRequest is null ? null : speakerRequest.Id.Value;

            context.Meetings.Update(meeting);

            await context.SaveChangesAsync(cancellationToken);

            await hubContext.Clients
               .Group($"meeting-{meeting.Id}")
               .OnMovedToNextSpeaker(agendaItem.Id, speakerRequestId);

            return Result.Success;
        }
    }
}