using MassTransit;

using MediatR;

using YourBrand.Payments.Domain;

namespace YourBrand.Payments.Application.Commands;

public record PostPayments(IEnumerable<PaymentDto> Payments) : IRequest
{
    public class Handler : IRequestHandler<PostPayments>
    {
        private readonly IPaymentsContext _context;
        private readonly IPublishEndpoint _publishEndpoint;

        public Handler(IPaymentsContext context, IPublishEndpoint publishEndpoint)
        {
            _context = context;
            _publishEndpoint = publishEndpoint;
        }

        public async Task<Unit> Handle(PostPayments request, CancellationToken cancellationToken)
        {
            /*
            foreach (var payment in request.Payments)
            {
                _context.Payments.Add(new Domain.Entities.Payment(
                    payment.Id,
                    payment.Date ?? DateTime.Now,
                    payment.Status,
                    payment.From,
                    payment.Reference,
                    payment.Currency,
                    payment.Amount));
            }

            await _context.SaveChangesAsync(cancellationToken);

            await _publishEndpoint.Publish(
                new Contracts.PaymentBatch(request.Payments.Select(t => new Contracts.Payment(t.Id, t.Date.GetValueOrDefault(), (Contracts.PaymentStatus)t.Status, t.From, t.Reference, t.Currency, t.Amount))));

            */

            return Unit.Value;
        }
    }
}