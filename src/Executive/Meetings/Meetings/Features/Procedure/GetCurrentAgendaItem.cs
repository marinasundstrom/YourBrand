using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Identity;
using YourBrand.Meetings.Features.Agendas;

namespace YourBrand.Meetings.Features.Procedure.Command;

public sealed record GetCurrentAgendaItem(string OrganizationId, int Id) : IRequest<Result<AgendaItemDto?>>
{
    public sealed class Handler(IApplicationDbContext context, IUserContext userContext) : IRequestHandler<GetCurrentAgendaItem, Result<AgendaItemDto?>>
    {
        public async Task<Result<AgendaItemDto?>> Handle(GetCurrentAgendaItem request, CancellationToken cancellationToken)
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

            var current = meeting.GetCurrentAgendaItem();

            return current?.ToDto();
        }
    }
}
