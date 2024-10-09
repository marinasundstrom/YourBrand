using FluentValidation;

using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Identity;

namespace YourBrand.Meetings.Features.Agendas.Command;

public record RemoveAgendaItem(string OrganizationId, int Id, string ItemId) : IRequest<Result<AgendaDto>>
{
    public class Validator : AbstractValidator<RemoveAgendaItem>
    {
        public Validator()
        {

        }
    }

    public class Handler(IApplicationDbContext context) : IRequestHandler<RemoveAgendaItem, Result<AgendaDto>>
    {
        public async Task<Result<AgendaDto>> Handle(RemoveAgendaItem request, CancellationToken cancellationToken)
        {
            var agenda = await context.Agendas
                .InOrganization(request.OrganizationId)
                .FirstOrDefaultAsync(x => x.Id == request.Id);

            if (agenda is null)
            {
                return Errors.Agendas.AgendaNotFound;
            }

            var agendaItem = agenda.Items.FirstOrDefault(x => x.Id == request.ItemId);

            if (agendaItem is null)
            {
                return Errors.Agendas.AgendaItemNotFound;
            }

            agenda.RemoveItem(agendaItem);

            context.Agendas.Update(agenda);

            await context.SaveChangesAsync(cancellationToken);

            agenda = await context.Agendas
                .InOrganization(request.OrganizationId)
                .FirstOrDefaultAsync(x => x.Id == agenda.Id!, cancellationToken);

            if (agenda is null)
            {
                return Errors.Agendas.AgendaNotFound;
            }

            return agenda.ToDto();
        }
    }
}