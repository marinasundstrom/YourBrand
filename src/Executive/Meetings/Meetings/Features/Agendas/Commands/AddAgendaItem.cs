using System;

using FluentValidation;

using MediatR;

using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

using YourBrand.Identity;
using YourBrand.Meetings.Features.Procedure;

namespace YourBrand.Meetings.Features.Agendas.Command;

public record AddAgendaItem(
    string OrganizationId,
    int Id,
    int Type,
    string Title,
    string Description,
    int? MotionId,
    int? Order,
    TimeSpan? EstimatedStartTime,
    TimeSpan? EstimatedEndTime,
    TimeSpan? EstimatedDuration) : IRequest<Result<AgendaItemDto>>
{
    public class Validator : AbstractValidator<AddAgendaItem>
    {
        public Validator()
        {
            //RuleFor(x => x.Title).NotEmpty().MaximumLength(60);
        }
    }

    public class Handler(IApplicationDbContext context, IHubContext<MeetingsProcedureHub, IMeetingsProcedureHubClient> hubContext) : IRequestHandler<AddAgendaItem, Result<AgendaItemDto>>
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

            var type = AgendaItemType.AllTypes.FirstOrDefault(x => x.Id == request.Type);

            if (type is null)
            {
                throw new Exception("Invalid type");
            }

            var agendaItem = agenda.AddItem(type, request.Title, request.Description);

            if (request.Order is not null)
            {
                agenda.ReorderAgendaItem(agendaItem, request.Order.GetValueOrDefault());
            }

            agendaItem.MotionId = request.MotionId;
            agendaItem.EstimatedStartTime = request.EstimatedStartTime;
            agendaItem.EstimatedEndTime = request.EstimatedEndTime;
            agendaItem.EstimatedDuration = request.EstimatedDuration;

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