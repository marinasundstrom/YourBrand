using MassTransit;

using MediatR;

using YourBrand.Payments.Domain;
using YourBrand.Payments.Domain.Enums;

namespace YourBrand.Payments.Application.Commands;

public record CreatePayment(string OrganizationId, string InvoiceId, string Currency, decimal Amount, DateTime DueDate, PaymentMethod PaymentMethod, string? Reference, string? Message) : IRequest
{
    public class Handler(IPaymentsContext context, IPublishEndpoint publishEndpoint) : IRequestHandler<CreatePayment>
    {
        public async Task Handle(CreatePayment request, CancellationToken cancellationToken)
        {
            context.Payments.Add(new Domain.Entities.Payment(
                    request.OrganizationId,
                    request.InvoiceId,
                    PaymentStatus.Created,
                    request.Currency,
                    request.Amount,
                    request.DueDate,
                    request.PaymentMethod,
                    request.Reference,
                    request.Message
                ));

            await context.SaveChangesAsync(cancellationToken);

        }
    }
}