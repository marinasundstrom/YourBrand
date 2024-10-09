using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Identity;
using YourBrand.Meetings.Features.Agendas;

namespace YourBrand.Meetings.Features.Command;

public sealed record StartAgendaItemDiscussion(string OrganizationId, int Id) : IRequest<Result>
{
    public sealed class Handler(IApplicationDbContext context, IUserContext userContext) : IRequestHandler<StartAgendaItemDiscussion, Result>
    {
        public async Task<Result> Handle(StartAgendaItemDiscussion request, CancellationToken cancellationToken)
        {
            var meeting = await context.Meetings
                .InOrganization(request.OrganizationId)
                .Include(x => x.Agenda)
                .ThenInclude(x => x.Items)
                .FirstOrDefaultAsync(x => x.Id == request.Id);

            if (meeting is null)
            {
                return Errors.Meetings.MeetingNotFound;
            }

            var participant = meeting.Participants.FirstOrDefault(x => x.UserId == userContext.UserId);

            if (participant is null)
                throw new UnauthorizedAccessException("You are not a participant of this meeting.");

            if (participant.Role != ParticipantRole.Chairperson)
                throw new UnauthorizedAccessException("Only the Chairperson can start the meeting.");

            var item = meeting.GetCurrentAgendaItem();

            item.StartDiscussion();

            context.Meetings.Update(meeting);

            await context.SaveChangesAsync(cancellationToken);

            return Result.Success;
        }
    }
}

