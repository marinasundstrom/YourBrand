using FluentValidation;

using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Identity;

namespace YourBrand.Meetings.Features.Agendas.Command;

public record EditAgendaItem(string OrganizationId, int Id, string ParticipantId, string Title, string Description) : IRequest<Result<AgendaItemDto>>
{
    public class Validator : AbstractValidator<EditAgendaItem>
    {
        public Validator()
        {
            RuleFor(x => x.Title).NotEmpty().MaximumLength(60);
        }
    }

    public class Handler(IApplicationDbContext context) : IRequestHandler<EditAgendaItem, Result<AgendaItemDto>>
    {
        public async Task<Result<AgendaItemDto>> Handle(EditAgendaItem request, CancellationToken cancellationToken)
        {
            var agenda = await context.Agendas
                .InOrganization(request.OrganizationId)
                .FirstOrDefaultAsync(x => x.Id == request.Id);

            if (agenda is null)
            {
                return Errors.Agendas.AgendaNotFound;
            }

            var agendaItem = agenda.Items.FirstOrDefault(x => x.Id == request.ParticipantId);

            if(agendaItem is  null) 
            {
                return Errors.Agendas.AgendaItemNotFound;
            }
        
            agendaItem.Title = request.Title;
            agendaItem.Description = request.Description;

            context.Agendas.Update(agenda);

            await context.SaveChangesAsync(cancellationToken);

            agenda = await context.Agendas
                .InOrganization(request.OrganizationId)
                .FirstOrDefaultAsync(x => x.Id == agenda.Id!, cancellationToken);

            if (agenda is null)
            {
                return Errors.Agendas.AgendaNotFound;
            }

            return agendaItem.ToDto();
        }
    }
}