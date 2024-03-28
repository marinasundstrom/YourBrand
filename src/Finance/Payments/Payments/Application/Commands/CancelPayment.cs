using MassTransit;

using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Payments.Domain;
using YourBrand.Payments.Domain.Enums;

namespace YourBrand.Payments.Application.Commands;

public record CancelPayment(string PaymentId) : IRequest
{
    public class Handler : IRequestHandler<CancelPayment>
    {
        private readonly IPaymentsContext _context;
        private readonly IPublishEndpoint _publishEndpoint;

        public Handler(IPaymentsContext context, IPublishEndpoint publishEndpoint)
        {
            _context = context;
            _publishEndpoint = publishEndpoint;
        }

        public async Task Handle(CancelPayment request, CancellationToken cancellationToken)
        {
            var payment = await _context.Payments.FirstOrDefaultAsync(p => p.Id == request.PaymentId);

            if(payment is null) 
            {
                throw new Exception();
            }

            payment.SetStatus(PaymentStatus.Cancelled);

            await _context.SaveChangesAsync(cancellationToken);

        }
    }
}