using System.Linq;

using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Identity;
using YourBrand.Meetings.Features.Agendas;

namespace YourBrand.Meetings.Features.Procedure.Command;

public sealed record GetCurrentAgendaItem(string OrganizationId, int Id) : IRequest<Result<AgendaItemDto?>>
{
    public sealed class Handler(IApplicationDbContext context, IUserContext userContext, IAgendaValidator agendaValidator) : IRequestHandler<GetCurrentAgendaItem, Result<AgendaItemDto?>>
    {
        public async Task<Result<AgendaItemDto?>> Handle(GetCurrentAgendaItem request, CancellationToken cancellationToken)
        {
            var meeting = await context.Meetings
                .InOrganization(request.OrganizationId)
                .AsSplitQuery()
                .Include(x => x.Attendees)
                    .ThenInclude(a => a.Functions)
                .Include(x => x.Agenda)
                    .ThenInclude(x => x.Items)
                        .ThenInclude(i => i.SubItems)
                .Include(x => x.Agenda)
                    .ThenInclude(x => x.Items)
                        .ThenInclude(i => i.Election)
                .Include(x => x.Agenda)
                    .ThenInclude(x => x.Items)
                        .ThenInclude(i => i.SubItems)
                            .ThenInclude(si => si.Election)
                .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

            if (meeting is null)
            {
                return Errors.Meetings.MeetingNotFound;
            }

            var agendaItem = meeting.GetCurrentAgendaItem();

            if (agendaItem is null)
            {
                return Errors.Meetings.NoActiveAgendaItem;
            }

            var validations = agendaValidator.Validate(meeting);

            return agendaItem?.ToDto(validations);
        }
    }
}