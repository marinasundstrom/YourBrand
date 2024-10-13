using MediatR;

using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

using YourBrand.Identity;
using YourBrand.Meetings.Features.Procedure.Command;

namespace YourBrand.Meetings.Features.Procedure.Discussions;

public sealed record RequestSpeakerTime(string OrganizationId, int Id) : IRequest<Result>
{
    public sealed class Handler(IApplicationDbContext context, IUserContext userContext, IHubContext<DiscussionsHub, IDiscussionsHubClient> hubContext) : IRequestHandler<RequestSpeakerTime, Result>
    {
        public async Task<Result> Handle(RequestSpeakerTime request, CancellationToken cancellationToken)
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

            if (!participant.HasVotingRights)
            {
                return Errors.Meetings.YouHaveNoVotingRights;
            }

            var agendaItem = meeting.GetCurrentAgendaItem();

            if (agendaItem is null)
            {
                return Errors.Meetings.NoActiveAgendaItem;
            }

            if (agendaItem.VotingSession is null)
            {
                return Errors.Meetings.NoOngoingVotingSession;
            }

            var speakerRequest = agendaItem.SpeakerSession!.AddSpeaker(participant);

            context.Meetings.Update(meeting);

            await context.SaveChangesAsync(cancellationToken);

            await hubContext.Clients
               .Group($"meeting-{meeting.Id}")
               .OnSpeakerRequestAdded(speakerRequest.Id, speakerRequest.ParticipantId);

            return Result.Success;
        }
    }
}
