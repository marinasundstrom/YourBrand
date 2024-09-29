using FluentValidation;

using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Ticketing.Application.Features.Tickets.Dtos;

namespace YourBrand.Ticketing.Application.Features.Tickets.Commands;

public record GetTicketCommentById(string OrganizationId, int TicketId, int Id) : IRequest<Result<TicketCommentDto>>
{
    public class Validator : AbstractValidator<GetTicketCommentById>
    {
        public Validator()
        {
            RuleFor(x => x.Id).NotEmpty();
        }
    }

    public class Handler(IApplicationDbContext context, IDtoComposer dtoComposer) : IRequestHandler<GetTicketCommentById, Result<TicketCommentDto>>
    {
        public async Task<Result<TicketCommentDto>> Handle(GetTicketCommentById request, CancellationToken cancellationToken)
        {
            var ticket = await context.Tickets.FirstOrDefaultAsync(x => x.Id == request.TicketId, cancellationToken);

            if (ticket is null)
                return null!;
                
            var ticketComment = await context.TicketComments
                .AsSplitQuery()
                .InOrganization(request.OrganizationId)
                .FirstOrDefaultAsync(x => x.Id == request.Id && x.TicketId == request.TicketId, cancellationToken);

            if (ticketComment is null)
            {
                return Result.Failure<TicketCommentDto>(Errors.Tickets.TicketCommentNotFound);
            }

            return Result.Success(await dtoComposer.ComposeTicketCommentDto(ticket, ticketComment, cancellationToken));
        }
    }
}