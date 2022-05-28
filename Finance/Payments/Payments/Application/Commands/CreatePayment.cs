using MassTransit;

using MediatR;

using YourBrand.Payments.Domain;
using YourBrand.Payments.Domain.Enums;

namespace YourBrand.Payments.Application.Commands;

public record CreatePayment(int InvoiceId, string Currency, decimal Amount, DateTime DueDate, PaymentMethod PaymentMethod, string? Reference, string? Message) : IRequest
{
    public class Handler : IRequestHandler<CreatePayment>
    {
        private readonly IPaymentsContext _context;
        private readonly IPublishEndpoint _publishEndpoint;

        public Handler(IPaymentsContext context, IPublishEndpoint publishEndpoint)
        {
            _context = context;
            _publishEndpoint = publishEndpoint;
        }

        public async Task<Unit> Handle(CreatePayment request, CancellationToken cancellationToken)
        {
            _context.Payments.Add(new Domain.Entities.Payment(
                    request.InvoiceId,
                    PaymentStatus.Created,
                    request.Currency,
                    request.Amount,
                    request.DueDate,
                    request.PaymentMethod,
                    request.Reference,
                    request.Message
                ));

            await _context.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}