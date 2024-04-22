using FluentValidation;

using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Sales.Persistence;


namespace YourBrand.Sales.Features.Subscriptions;

public record GetSubscriptionByNo(string OrganizationId, int SubscriptionNo) : IRequest<Result<SubscriptionDto>>
{
    public class Validator : AbstractValidator<GetSubscriptionByNo>
    {
        public Validator()
        {
            RuleFor(x => x.SubscriptionNo);
        }
    }

    public class Handler(SalesContext salesContext) : IRequestHandler<GetSubscriptionByNo, Result<SubscriptionDto>>
    {
        public async Task<Result<SubscriptionDto>> Handle(GetSubscriptionByNo request, CancellationToken cancellationToken)
        {
            var subscription = await salesContext.Subscriptions
                .Where(x => x.OrganizationId == request.OrganizationId)
                .Include(x => x.SubscriptionPlan)
                .Include(x => x.Order)
                .FirstOrDefaultAsync(c => c.SubscriptionNo == request.SubscriptionNo);

            if (subscription is null)
            {
                throw new Exception();
            }

            return subscription.ToDto();
        }
    }
}