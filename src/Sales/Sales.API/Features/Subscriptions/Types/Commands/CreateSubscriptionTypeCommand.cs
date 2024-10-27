using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Sales.Features.SubscriptionManagement.Subscriptions;

namespace YourBrand.Sales.Features.SubscriptionManagement.Subscriptions.Types.Commands;

public record CreateSubscriptionTypeCommand(string OrganizationId, string Name, string Handle, string? Description) : IRequest<SubscriptionTypeDto>
{
    public class CreateSubscriptionTypeCommandHandler(ISalesContext context) : IRequestHandler<CreateSubscriptionTypeCommand, SubscriptionTypeDto>
    {
        private readonly ISalesContext context = context;

        public async Task<SubscriptionTypeDto> Handle(CreateSubscriptionTypeCommand request, CancellationToken cancellationToken)
        {
            var subscriptionType = await context.SubscriptionTypes.FirstOrDefaultAsync(i => i.Name == request.Name, cancellationToken);

            if (subscriptionType is not null) throw new Exception();

            int subscriptionTypeNo = 1;

            try
            {
                subscriptionTypeNo = await context.SubscriptionTypes
                    .Where(x => x.OrganizationId == request.OrganizationId)
                    .MaxAsync(x => x.Id, cancellationToken) + 1;
            }
            catch { }

            subscriptionType = new Domain.Entities.SubscriptionType(subscriptionTypeNo, request.Name, request.Handle, request.Description);
            subscriptionType.OrganizationId = request.OrganizationId;

            context.SubscriptionTypes.Add(subscriptionType);

            await context.SaveChangesAsync(cancellationToken);

            return subscriptionType.ToDto();
        }
    }
}