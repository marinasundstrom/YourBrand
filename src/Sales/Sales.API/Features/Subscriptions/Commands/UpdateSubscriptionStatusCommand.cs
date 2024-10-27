using FluentValidation;

using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Sales.Persistence;

namespace YourBrand.Sales.Features.SubscriptionManagement;

public sealed record UpdateSubscriptionStatus(string OrganizationId, Guid Id, int StatusId) : IRequest<Result>
{
    public sealed class Validator : AbstractValidator<UpdateSubscriptionStatus>
    {
        public Validator()
        {
            RuleFor(x => x.Id).NotEmpty();
        }
    }

    public sealed class Handler(SalesContext salesContext) : IRequestHandler<UpdateSubscriptionStatus, Result>
    {
        public async Task<Result> Handle(UpdateSubscriptionStatus request, CancellationToken cancellationToken)
        {
            var subscription = await salesContext.Subscriptions
                            //.InOrganization(request.OrganizationId)
                            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

            if (subscription is null)
            {
                return Errors.Subscriptions.SubscriptionNotFound;
            }

            subscription.UpdateStatus(request.StatusId);
            await salesContext.SaveChangesAsync(cancellationToken);

            return Results.Success;
        }
    }
}