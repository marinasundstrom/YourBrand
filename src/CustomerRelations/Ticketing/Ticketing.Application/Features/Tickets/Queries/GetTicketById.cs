using FluentValidation;

using MediatR;

using YourBrand.Ticketing.Application.Features.Tickets.Dtos;

namespace YourBrand.Ticketing.Application.Features.Tickets.Queries;

public record GetTicketById(string OrganizationId, int Id) : IRequest<Result<TicketDto>>
{
    public class Validator : AbstractValidator<GetTicketById>
    {
        public Validator()
        {
            RuleFor(x => x.Id).NotEmpty();
        }
    }

    public class Handler(ITicketRepository ticketRepository, IDtoComposer dtoComposer) : IRequestHandler<GetTicketById, Result<TicketDto>>
    {
        public async Task<Result<TicketDto>> Handle(GetTicketById request, CancellationToken cancellationToken)
        {
            var ticket = await ticketRepository.FindByIdAsync(request.Id, cancellationToken);

            if (ticket is null)
            {
                return Result.Failure<TicketDto>(Errors.Tickets.TicketNotFound);
            }

            return Result.Success(await dtoComposer.ComposeTicketDto(ticket, cancellationToken));
        }
    }
}