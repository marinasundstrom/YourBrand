using YourBrand.Invoices.Application.Common.Models;
using YourBrand.Invoices.Contracts;
using YourBrand.Invoices.Domain;
using YourBrand.Invoices.Domain.Enums;
using YourBrand.Invoices.Domain.Events;

using MassTransit;

using MediatR;

using Microsoft.EntityFrameworkCore;
using YourBrand.Payments.Client;

namespace YourBrand.Invoices.Application.Events;

public class InvoiceStatusChangedHandler : INotificationHandler<DomainEventNotification<InvoiceStatusChanged>>
{
    private readonly IInvoicesContext _context;
    private readonly IPaymentsClient _paymentsClient;
    private readonly IPublishEndpoint _publishEndpoint;

    public InvoiceStatusChangedHandler(IInvoicesContext context, IPaymentsClient paymentsClient, IPublishEndpoint publishEndpoint)
    {
        _context = context;
        _paymentsClient = paymentsClient;
        _publishEndpoint = publishEndpoint;
    }

    public async Task Handle(DomainEventNotification<InvoiceStatusChanged> notification, CancellationToken cancellationToken)
    {
        var invoice = await _context.Invoices
            .Include(i => i.Items)
            .FirstOrDefaultAsync(i => i.Id == notification.DomainEvent.InvoiceId);

        if(invoice is not null) 
        {
            if (invoice.Status == InvoiceStatus.Sent)
            {
                await _publishEndpoint.Publish(new InvoicesBatch(new[]
                {
                    new Contracts.Invoice(invoice.Id)
                }));

                var dueDate = TimeZoneInfo.ConvertTimeToUtc( DateTime.Now.AddDays(30), TimeZoneInfo.Local);

                await _paymentsClient.CreatePaymentAsync(new CreatePayment()
                {
                    InvoiceId = invoice.Id,
                    Currency = "SEK",
                    Amount = invoice.Total,
                    PaymentMethod = PaymentMethod.PlusGiro,
                    DueDate = dueDate,
                    Reference = Guid.NewGuid().ToUrlFriendlyString(),
                    Message = $"Betala faktura #{invoice.Id}",
                });

                await CreateRotRutCase(invoice, cancellationToken);
            }
            else if(invoice.Status == InvoiceStatus.Paid)
            {
                await MarkRotRutCaseAsPaid(invoice, cancellationToken);
            }
            else if(invoice.Status == InvoiceStatus.Void)
            {
                await DeleteRotRutCase(invoice, cancellationToken);
            }

        }
    }

    private async Task DeleteRotRutCase(Domain.Entities.Invoice? invoice, CancellationToken cancellationToken)
    {
        var domesticServices = invoice?.DomesticService;
        if (domesticServices is not null)
        {
            var rotRutCase = await _context.RotRutCases.FirstAsync(x => x.InvoiceId == invoice.Id, cancellationToken);

            _context.RotRutCases.Remove(rotRutCase);

            await _context.SaveChangesAsync(cancellationToken);
        }
    }

    private async Task MarkRotRutCaseAsPaid(Domain.Entities.Invoice? invoice, CancellationToken cancellationToken)
    {
        var domesticServices = invoice?.DomesticService;
        if (domesticServices is not null)
        {
            var rotRutCase = await _context.RotRutCases.FirstAsync(x => x.InvoiceId == invoice!.Id, cancellationToken);
            rotRutCase.Status = Domain.Entities.RotRutCaseStatus.InvoicePaid;

            await _context.SaveChangesAsync(cancellationToken);
        }
    }

    private async Task CreateRotRutCase(Domain.Entities.Invoice? invoice, CancellationToken cancellationToken)
    {
        // If as ROT/RUT
        // Create

        var domesticServices = invoice?.DomesticService;
        if (domesticServices is not null)
        {

            var itemsWithoutHouseholdServices = invoice.Items.Where(x => x.ProductType == ProductType.Good);
            var itemsWithHouseholdServices = invoice.Items.Where(x => x.ProductType == ProductType.Service && x.DomesticService is not null);

            var hours = itemsWithHouseholdServices.Sum(x => x.Quantity);
            var laborCost = itemsWithHouseholdServices.Sum(x => x.LineTotal);
            var materialCost = itemsWithoutHouseholdServices.Sum(x => x.LineTotal);

            decimal requestedAmount = 0;
            if (domesticServices.Kind == Domain.Entities.DomesticServiceKind.HomeRepairAndMaintenanceServiceType)
            {
                requestedAmount = laborCost * (decimal)0.30; //invoice.DomesticServiceDeductibleAmount
            }
            else if (domesticServices.Kind == Domain.Entities.DomesticServiceKind.HouseholdService)
            {
                requestedAmount = laborCost * (decimal)0.50;
            }

            DateTime paymentDate = DateTime.Now;
            decimal paidAmount = invoice.Total;
            decimal otherCosts = 0m;

            var rotRutCase =
                new Domain.Entities.RotRutCase(domesticServices.Kind, invoice.DomesticService!.Buyer, paymentDate, laborCost, paidAmount, requestedAmount, invoice.Id, otherCosts, hours, materialCost, null);

            if (domesticServices.Kind == Domain.Entities.DomesticServiceKind.HomeRepairAndMaintenanceServiceType)
            {
                var first = itemsWithHouseholdServices.First();

                rotRutCase.Rot = new Domain.Entities.Rot() {
                    ServiceType = first.DomesticService!.HomeRepairAndMaintenanceServiceType,
                    PropertyDesignation =  invoice.DomesticService!.PropertyDetails!.PropertyDesignation,
                    ApartmentNo =  invoice.DomesticService!.PropertyDetails!.ApartmentNo,
                    OrganizationNo =  invoice.DomesticService!.PropertyDetails!.OrganizationNo
                };
            }
            else if (domesticServices.Kind == Domain.Entities.DomesticServiceKind.HouseholdService)
            {
                var first = itemsWithHouseholdServices.First();

                rotRutCase.Rut = new Domain.Entities.Rut() {
                    ServiceType = first.DomesticService!.HouseholdServiceType
                };
            }

            _context.RotRutCases.Add(rotRutCase);

            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}