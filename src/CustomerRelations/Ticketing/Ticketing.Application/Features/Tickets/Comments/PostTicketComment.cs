using FluentValidation;

using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Identity;
using YourBrand.Tenancy;
using YourBrand.Ticketing.Application.Features.Tickets.Dtos;

namespace YourBrand.Ticketing.Application.Features.Tickets.Commands;

public sealed record PostTicketComment(string OrganizationId, int Id, string Text) : IRequest<Result<TicketCommentDto>>
{
    public sealed class Validator : AbstractValidator<PostTicketComment>
    {
        public Validator()
        {
            RuleFor(x => x.Id).NotEmpty();

            RuleFor(x => x.Text).MaximumLength(1024);
        }
    }

    public sealed class Handler(IDtoComposer dtoComposer, IApplicationDbContext context, ITicketRepository ticketRepository, IUnitOfWork unitOfWork, 
        ITenantContext tenantContext, IUserContext userContext) : IRequestHandler<PostTicketComment, Result<TicketCommentDto>>
    {
        public async Task<Result<TicketCommentDto>> Handle(PostTicketComment request, CancellationToken cancellationToken)
        {
            var ticket = await ticketRepository.FindByIdAsync(request.Id, cancellationToken);

            if (ticket is null)
            {
                return Result.Failure<TicketCommentDto>(Errors.Tickets.TicketNotFound);
            }

            int ticketCommentId = 1;

            try
            {
                ticketCommentId = await context.TicketComments
                    .InOrganization(request.OrganizationId)
                    .MaxAsync(x => x.Id, cancellationToken) + 1;
            }
            catch { }

            var ticketComment = new TicketComment(ticketCommentId)
            {
                OrganizationId = request.OrganizationId,
                TicketId = ticket.Id,
                Text = request.Text
            };

            context.TicketComments.Add(ticketComment);

            ticketComment.AddDomainEvent(new TicketCommentAdded(tenantContext.TenantId.GetValueOrDefault(), request.OrganizationId, ticket.Id, ticketCommentId));

            var participant = ticket.Participants.FirstOrDefault(x => x.UserId == userContext.UserId);

            ticket.LastModifiedBy = participant;
            ticket.LastModified = DateTimeOffset.UtcNow;

            ticketComment.CreatedById = participant!.Id;
            ticketComment.Created = DateTimeOffset.UtcNow;

            await context.SaveChangesAsync(cancellationToken);

            return Result.Success(await dtoComposer.ComposeTicketCommentDto(ticket, ticketComment, cancellationToken));
        }
    }
}