using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Payments.Domain;
using YourBrand.Payments.Domain.Enums;

namespace YourBrand.Payments.Application.Commands;

public record SetPaymentStatus(string OrganizationId, string PaymentId, PaymentStatus Status) : IRequest
{
    public class Handler(IPaymentsContext context) : IRequestHandler<SetPaymentStatus>
    {
        public async Task Handle(SetPaymentStatus request, CancellationToken cancellationToken)
        {
            var payment = await context.Payments
                .InOrganization(request.OrganizationId)
                .FirstAsync(x => x.Id == request.PaymentId, cancellationToken);

            payment.SetStatus(request.Status);

            await context.SaveChangesAsync(cancellationToken);

        }
    }
}