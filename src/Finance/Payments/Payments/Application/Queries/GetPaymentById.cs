
using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Payments.Domain;

namespace YourBrand.Payments.Application.Queries;

public record GetPaymentById(string OrganizationId, string Id) : IRequest<PaymentDto?>
{
    public class Handler(IPaymentsContext context) : IRequestHandler<GetPaymentById, PaymentDto?>
    {
        public async Task<PaymentDto?> Handle(GetPaymentById request, CancellationToken cancellationToken)
        {
            var payment = await context.Payments
                .InOrganization(request.OrganizationId)
                .FirstOrDefaultAsync(p => p.Id == request.Id);

            return payment?.ToDto();
        }
    }
}