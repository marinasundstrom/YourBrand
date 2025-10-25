using System.Linq;
using YourBrand.Meetings.Domain.Entities;

using MediatR;

using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

using YourBrand.Identity;
using YourBrand.Meetings.Features.Agendas;
using YourBrand.Meetings.Features.Procedure.Command;
using YourBrand.Meetings.Features.Procedure.Discussions;

namespace YourBrand.Meetings.Features.Procedure.Chairman;

public sealed record ExtendSpeakerTime(string OrganizationId, int Id, string AgendaItemId, string SpeakerRequestId, int AdditionalSeconds) : IRequest<Result>
{
    public sealed class Handler(IApplicationDbContext context, IUserContext userContext, IHubContext<MeetingsProcedureHub, IMeetingsProcedureHubClient> hubContext)
        : IRequestHandler<ExtendSpeakerTime, Result>
    {
        public async Task<Result> Handle(ExtendSpeakerTime request, CancellationToken cancellationToken)
        {
            if (request.AdditionalSeconds <= 0)
            {
                return Errors.Meetings.InvalidAdditionalSpeakingTime;
            }

            var meeting = await context.Meetings
                .InOrganization(request.OrganizationId)
                .Include(x => x.Agenda)
                .ThenInclude(x => x.Items.OrderBy(x => x.Order))
                .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

            if (meeting is null)
            {
                return Errors.Meetings.MeetingNotFound;
            }

            var attendee = meeting.GetAttendeeByUserId(userContext.UserId);

            if (attendee is null)
            {
                return Errors.Meetings.YouAreNotAttendeeOfMeeting;
            }

            var chairFunction = meeting.GetChairpersonFunction(attendee);

            if (chairFunction is null)
            {
                return Errors.Meetings.OnlyChairpersonCanManageSpeakerQueue;
            }

            var agendaItem = meeting.GetAgendaItem(request.AgendaItemId);

            if (agendaItem is null)
            {
                return Errors.Meetings.AgendaItemNotFound;
            }

            if (agendaItem.Discussion is null)
            {
                return Errors.Meetings.NoOngoingDiscussionSession;
            }

            try
            {
                chairFunction.ExtendSpeakerTime(agendaItem, request.SpeakerRequestId, TimeSpan.FromSeconds(request.AdditionalSeconds));
            }
            catch (InvalidOperationException)
            {
                return Errors.Meetings.SpeakerRequestNotFound;
            }

            context.Meetings.Update(meeting);

            await context.SaveChangesAsync(cancellationToken);

            var speakerRequests = agendaItem.Discussion.GetOrderedSpeakerQueue()
                .Concat(agendaItem.Discussion.CurrentSpeaker is null
                    ? Enumerable.Empty<SpeakerRequest>()
                    : new[] { agendaItem.Discussion.CurrentSpeaker });

            var updatedSpeaker = speakerRequests.FirstOrDefault(x => x.Id == request.SpeakerRequestId);

            await hubContext.Clients
                .Group($"meeting-{meeting.Id}")
                .OnSpeakerTimeExtended(
                    agendaItem.Id,
                    request.SpeakerRequestId,
                    (int?)updatedSpeaker?.AllocatedSpeakingTime?.TotalSeconds);

            return Result.Success;
        }
    }
}
