using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Identity;
using YourBrand.Meetings.Features.Agendas;

namespace YourBrand.Meetings.Features.Command;

public sealed record GetMeetingAgenda(string OrganizationId, int Id) : IRequest<Result<AgendaDto>>
{
    public sealed class Handler(IApplicationDbContext context, IUserContext userContext) : IRequestHandler<GetMeetingAgenda, Result<AgendaDto>>
    {
        public async Task<Result<AgendaDto>> Handle(GetMeetingAgenda request, CancellationToken cancellationToken)
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

            if (meeting.Agenda is null)
            {
                return Errors.Agendas.AgendaNotFound;
            }

            return meeting.Agenda.ToDto();
        }
    }
}
