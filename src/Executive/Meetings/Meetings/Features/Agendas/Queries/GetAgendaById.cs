using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Meetings.Domain;
using YourBrand.Meetings.Features;
using YourBrand.Meetings.Models;

namespace YourBrand.Meetings.Features.Agendas.Queries;

public record GetAgendaById(string OrganizationId, int Id) : IRequest<Result<AgendaDto>>
{
    public class Handler(IApplicationDbContext context) : IRequestHandler<GetAgendaById, Result<AgendaDto>>
    {
        public async Task<Result<AgendaDto>> Handle(GetAgendaById request, CancellationToken cancellationToken)
        {
            var agenda = await context.Agendas
                .InOrganization(request.OrganizationId)
                .AsNoTracking()
                .Include(x => x.Items.OrderBy(x => x.Order))
                .FirstOrDefaultAsync(x => x.Id == request.Id);

            if (agenda is null)
            {
                return Errors.Agendas.AgendaNotFound;
            }

            return agenda.ToDto();
        }
    }
}