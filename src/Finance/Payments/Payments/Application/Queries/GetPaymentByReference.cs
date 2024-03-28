using MassTransit;

using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Payments.Domain;
using YourBrand.Payments.Domain.Enums;

namespace YourBrand.Payments.Application.Queries;

public record GetPaymentByReference(string Reference) : IRequest<PaymentDto?>
{
    public class Handler : IRequestHandler<GetPaymentByReference, PaymentDto?>
    {
        private readonly IPaymentsContext _context;

        public Handler(IPaymentsContext context)
        {
            _context = context;
        }

        public async Task<PaymentDto?> Handle(GetPaymentByReference request, CancellationToken cancellationToken)
        {
            var payment = await _context.Payments.FirstOrDefaultAsync(p => p.Reference == request.Reference);

            return payment?.ToDto();
        }
    }
}