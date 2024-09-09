using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Payments.Domain;

namespace YourBrand.Payments.Application.Queries;

public record GetPaymentByReference(string OrganizationId, string Reference) : IRequest<PaymentDto?>
{
    public class Handler(IPaymentsContext context) : IRequestHandler<GetPaymentByReference, PaymentDto?>
    {
        public async Task<PaymentDto?> Handle(GetPaymentByReference request, CancellationToken cancellationToken)
        {
            var payment = await context.Payments
                .InOrganization(request.OrganizationId)
                .FirstOrDefaultAsync(p => p.Reference == request.Reference);

            return payment?.ToDto();
        }
    }
}