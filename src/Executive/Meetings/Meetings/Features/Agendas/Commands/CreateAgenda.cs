using System;

using FluentValidation;

using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Identity;

namespace YourBrand.Meetings.Features.Agendas.Command;

public sealed record CreateAgendaItemDto(
    int Type,
    string Title,
    string Description,
    TimeSpan? EstimatedStartTime = null,
    TimeSpan? EstimatedEndTime = null,
    TimeSpan? EstimatedDuration = null);

public record CreateAgenda(string OrganizationId, int MeetingId, IEnumerable<CreateAgendaItemDto> Items) : IRequest<Result<AgendaDto>>
{
    public class Validator : AbstractValidator<CreateAgenda>
    {
        public Validator()
        {
            //RuleFor(x => x.Title).NotEmpty().MaximumLength(60);

            //RuleFor(x => x.Items).NotEmpty();
        }
    }

    public class Handler(IApplicationDbContext context) : IRequestHandler<CreateAgenda, Result<AgendaDto>>
    {
        public async Task<Result<AgendaDto>> Handle(CreateAgenda request, CancellationToken cancellationToken)
        {
            int id = 1;

            try
            {
                id = await context.Agendas
                    .InOrganization(request.OrganizationId)
                    .MaxAsync(x => x.Id, cancellationToken) + 1;
            }
            catch { }

            var agenda = new Agenda(id);
            agenda.OrganizationId = request.OrganizationId;
            agenda.MeetingId = request.MeetingId;

            foreach (var agendaItem in request.Items)
            {
                var type = AgendaItemType.AllTypes.FirstOrDefault(x => x.Id == agendaItem.Type);

                if (type is null)
                {
                    throw new Exception("Invalid type");
                }

                var item = agenda.AddItem(type, agendaItem.Title, agendaItem.Description);
                item.EstimatedStartTime = agendaItem.EstimatedStartTime;
                item.EstimatedEndTime = agendaItem.EstimatedEndTime;
                item.EstimatedDuration = agendaItem.EstimatedDuration;
            }

            context.Agendas.Add(agenda);

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