using MediatR;

using Microsoft.EntityFrameworkCore;

namespace YourBrand.Sales.Features.SubscriptionManagement.Subscriptions.Types.Commands;

public record DeleteSubscriptionTypeCommand(string OrganizationId, int Id) : IRequest
{
    public class DeleteSubscriptionTypeCommandHandler(ISalesContext context) : IRequestHandler<DeleteSubscriptionTypeCommand>
    {
        private readonly ISalesContext context = context;

        public async Task Handle(DeleteSubscriptionTypeCommand request, CancellationToken cancellationToken)
        {
            var subscriptionType = await context.SubscriptionTypes
                .Where(x => x.OrganizationId == request.OrganizationId)
                .FirstOrDefaultAsync(i => i.Id == request.Id, cancellationToken);

            if (subscriptionType is null) throw new Exception();

            context.SubscriptionTypes.Remove(subscriptionType);

            await context.SaveChangesAsync(cancellationToken);
        }
    }
}