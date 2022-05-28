
using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Payments.Domain;

namespace YourBrand.Payments.Application.Queries;

public record GetPaymentById(string Id) : IRequest<PaymentDto?>
{
    public class Handler : IRequestHandler<GetPaymentById, PaymentDto?>
    {
        private readonly IPaymentsContext _context;

        public Handler(IPaymentsContext context)
        {
            _context = context;
        }

        public async Task<PaymentDto?> Handle(GetPaymentById request, CancellationToken cancellationToken)
        {
            var payment = await _context.Payments.FirstOrDefaultAsync(p => p.Id == request.Id);

            return payment?.ToDto();
        }
    }
}
