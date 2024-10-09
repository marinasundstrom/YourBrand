using FluentValidation;

using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Identity;

namespace YourBrand.Meetings.Features.Agendas.Command;

public record AddAgendaItem(string OrganizationId, int Id, string Title, string Description, int? MotionId) : IRequest<Result<AgendaItemDto>>
{
    public class Validator : AbstractValidator<AddAgendaItem>
    {
        public Validator()
        {
            //RuleFor(x => x.Title).NotEmpty().MaximumLength(60);
        }
    }

    public class Handler(IApplicationDbContext context) : IRequestHandler<AddAgendaItem, Result<AgendaItemDto>>
    {
        public async Task<Result<AgendaItemDto>> Handle(AddAgendaItem request, CancellationToken cancellationToken)
        {
            var agenda = await context.Agendas
                .InOrganization(request.OrganizationId)
                .FirstOrDefaultAsync(x => x.Id == request.Id);

            if (agenda is null)
            {
                return Errors.Agendas.AgendaNotFound;
            }

            var agendaItem = agenda.AddItem(request.Title, request.Description);

            agendaItem.MotionId = request.MotionId;

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