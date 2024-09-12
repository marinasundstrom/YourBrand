﻿using FluentValidation;

using MediatR;

namespace YourBrand.Ticketing.Application.Features.Tickets.Commands;

public sealed record UpdateEstimatedHours(string OrganizationId, int Id, double? Hours) : IRequest<Result>
{
    public sealed class Validator : AbstractValidator<UpdateEstimatedHours>
    {
        public Validator()
        {
            RuleFor(x => x.Id).NotEmpty();
        }
    }

    public sealed class Handler(ITicketRepository ticketRepository, IUnitOfWork unitOfWork) : IRequestHandler<UpdateEstimatedHours, Result>
    {
        public async Task<Result> Handle(UpdateEstimatedHours request, CancellationToken cancellationToken)
        {
            var ticket = await ticketRepository.FindByIdAsync(request.Id, cancellationToken);

            if (ticket is null)
            {
                return Result.Failure(Errors.Tickets.TicketNotFound);
            }

            ticket.UpdateEstimatedHours(request.Hours);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}