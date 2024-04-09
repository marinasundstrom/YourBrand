using MassTransit;

using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Payments.Domain;
using YourBrand.Payments.Domain.Enums;

namespace YourBrand.Payments.Application.Commands;

public record CancelPayment(string PaymentId) : IRequest
{
    public class Handler(IPaymentsContext context, IPublishEndpoint publishEndpoint) : IRequestHandler<CancelPayment>
    {
        public async Task Handle(CancelPayment request, CancellationToken cancellationToken)
        {
            var payment = await context.Payments.FirstOrDefaultAsync(p => p.Id == request.PaymentId);

            if (payment is null)
            {
                throw new Exception();
            }

            payment.SetStatus(PaymentStatus.Cancelled);

            await context.SaveChangesAsync(cancellationToken);

        }
    }
}