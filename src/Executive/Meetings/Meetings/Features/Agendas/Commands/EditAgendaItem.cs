using FluentValidation;

using MediatR;

using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

using YourBrand.Identity;
using YourBrand.Meetings.Features.Procedure.Command;

namespace YourBrand.Meetings.Features.Agendas.Command;

public record EditAgendaItem(string OrganizationId, int Id, string ItemId, AgendaItemType Type, string Title, string Description, int? MotionId) : IRequest<Result<AgendaItemDto>>
{
    public class Validator : AbstractValidator<EditAgendaItem>
    {
        public Validator()
        {
            RuleFor(x => x.Title).NotEmpty().MaximumLength(60);
        }
    }

    public class Handler(IApplicationDbContext context, IHubContext<MeetingsProcedureHub, IMeetingsProcedureHubClient> hubContext) : IRequestHandler<EditAgendaItem, Result<AgendaItemDto>>
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

            var agendaItem = agenda.Items.FirstOrDefault(x => x.Id == request.ItemId);

            if(agendaItem is  null) 
            {
                return Errors.Agendas.AgendaItemNotFound;
            }
        
            agendaItem.Type = request.Type;
            agendaItem.Title = request.Title;
            agendaItem.Description = request.Description;
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

            await hubContext.Clients
                .Group($"meeting-{agenda.MeetingId}")
                .OnAgendaUpdated();

            return agendaItem.ToDto();
        }
    }
}