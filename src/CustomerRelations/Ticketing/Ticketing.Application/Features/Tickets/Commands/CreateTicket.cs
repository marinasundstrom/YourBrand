using FluentValidation;

using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Tenancy;
using YourBrand.Identity;

using YourBrand.Ticketing.Application.Features.Tickets.Dtos;

namespace YourBrand.Ticketing.Application.Features.Tickets.Commands;

public sealed record CreateTicket(string OrganizationId, string Title, string? Description, int Status, string? AssigneeUserId, double? EstimatedHours, double? RemainingHours) : IRequest<Result<TicketDto>>
{
    public sealed class Validator : AbstractValidator<CreateTicket>
    {
        public Validator()
        {
            RuleFor(x => x.Title).NotEmpty().MaximumLength(60);

            RuleFor(x => x.Description).MaximumLength(240);
        }
    }

    public sealed class Handler(ITicketRepository ticketRepository, IUnitOfWork unitOfWork, IApplicationDbContext context, IUserContext userContext, IDomainEventDispatcher domainEventDispatcher, ITenantContext tenantContext) : IRequestHandler<CreateTicket, Result<TicketDto>>
    {
        public async Task<Result<TicketDto>> Handle(CreateTicket request, CancellationToken cancellationToken)
        {
            int ticketId = 1;

            try
            {
                ticketId = await context.Tickets
                    .InOrganization(request.OrganizationId)
                    .MaxAsync(x => x.Id, cancellationToken) + 1;
            }
            catch { }

            var ticket = new Ticket(ticketId, request.Title, request.Description!);
            ticket.OrganizationId = request.OrganizationId;
            ticket.TypeId = 1;
            ticket.CategoryId = 1;

            ticket.Status = await context.TicketStatuses.FirstAsync(s => s.Id == request.Status, cancellationToken);

            ticket.UpdateEstimatedHours(request.EstimatedHours);
            ticket.UpdateRemainingHours(request.RemainingHours);

            var creatorParticipant = new TicketParticipant 
            {
                OrganizationId = request.OrganizationId,
                Name = null,
                UserId = userContext.UserId
            };

            ticketRepository.Add(ticket);

            await unitOfWork.SaveChangesAsync(cancellationToken);

            if (request.AssigneeUserId is not null)
            {
                var participant = ticket.Participants.FirstOrDefault(x => x.UserId == request.AssigneeUserId);

                if (participant is null)
                {
                    participant = new TicketParticipant
                    {
                        OrganizationId = request.OrganizationId,
                        Name = null,
                        UserId = request.AssigneeUserId
                    };
                }

                ticket.Participants.Add(creatorParticipant);

                await unitOfWork.SaveChangesAsync(cancellationToken);

                ticket.UpdateAssignee(participant.Id);

                ticket.ClearDomainEvents();
            }

            ticket.Participants.Add(creatorParticipant);

            await unitOfWork.SaveChangesAsync(cancellationToken);

            ticket.CreatedBy = creatorParticipant;
            ticket.Created = DateTimeOffset.UtcNow;

            await unitOfWork.SaveChangesAsync(cancellationToken);

            await domainEventDispatcher.Dispatch(new TicketCreated(tenantContext.TenantId.GetValueOrDefault(), request.OrganizationId, ticket.Id), cancellationToken);

            ticket = await ticketRepository.GetAll()
                .OrderBy(i => i.Id)
                .Include(i => i.Status)
                .Include(i => i.Type)
                .Include(i => i.Assignee)
                .Include(i => i.CreatedBy)
                .Include(i => i.LastModifiedBy)
                .LastAsync(cancellationToken);

            return Result.Success(ticket!.ToDto());
        }
    }
}