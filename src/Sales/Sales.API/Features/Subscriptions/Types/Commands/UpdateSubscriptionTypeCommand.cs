using MediatR;

using Microsoft.EntityFrameworkCore;

namespace YourBrand.Sales.Features.SubscriptionManagement.Subscriptions.Types.Commands;

public record UpdateSubscriptionTypeCommand(string OrganizationId, int Id, string Name, string Handle, string? Description) : IRequest
{
    public class UpdateSubscriptionTypeCommandHandler(ISalesContext context) : IRequestHandler<UpdateSubscriptionTypeCommand>
    {
        private readonly ISalesContext context = context;

        public async Task Handle(UpdateSubscriptionTypeCommand request, CancellationToken cancellationToken)
        {
            var subscriptionType = await context.SubscriptionTypes
                    .Where(x => x.OrganizationId == request.OrganizationId)
                    .FirstOrDefaultAsync(i => i.Id == request.Id, cancellationToken);

            if (subscriptionType is null) throw new Exception();

            subscriptionType.Name = request.Name;
            subscriptionType.Handle = request.Handle;
            subscriptionType.Description = request.Description;

            await context.SaveChangesAsync(cancellationToken);
        }
    }
}