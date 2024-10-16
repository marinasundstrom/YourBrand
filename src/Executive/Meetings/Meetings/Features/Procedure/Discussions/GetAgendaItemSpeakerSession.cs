using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Identity;
using YourBrand.Meetings.Features.Agendas;

namespace YourBrand.Meetings.Features.Procedure.Discussions;

public sealed record GetAgendaItemSpeakerSession(string OrganizationId, int Id) : IRequest<Result<SpeakerSessionDto?>>
{
    public sealed class Handler(IApplicationDbContext context, IUserContext userContext) : IRequestHandler<GetAgendaItemSpeakerSession, Result<SpeakerSessionDto?>>
    {
        public async Task<Result<SpeakerSessionDto?>> Handle(GetAgendaItemSpeakerSession request, CancellationToken cancellationToken)
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

            var agendaItem = meeting.GetCurrentAgendaItem();

            if (agendaItem is null)
            {
                return Errors.Meetings.NoActiveAgendaItem;
            }

            if (agendaItem.SpeakerSession is null) 
            {
                return Errors.Meetings.NoOngoingSpeakerSession;
            }

            return agendaItem.SpeakerSession?.ToDto();
        }
    }
}
