using YourBrand.Accounting.Client;

using YourBrand.Documents.Client;

using YourBrand.Invoices.Client;
using YourBrand.Invoices.Contracts;

using MassTransit;
using YourBrand.RotRutService.Domain;

namespace YourBrand.RotRutService.Consumers;

public class InvoicePaidConsumer : IConsumer<InvoicePaid>
{
    private readonly IRotRutContext _context;
    private readonly IVerificationsClient _verificationsClient;
    private readonly IInvoicesClient _invoicesClient;

    public InvoicePaidConsumer(IRotRutContext context, IVerificationsClient verificationsClient,
        IInvoicesClient invoicesClient)
    {
        _context = context;
        _verificationsClient = verificationsClient;
        _invoicesClient = invoicesClient;
    }

    public async Task Consume(ConsumeContext<InvoicePaid> context)
    {
        var invoice = await _invoicesClient.GetInvoiceAsync(context.Message.Id, context.CancellationToken);

        await CreateRotRutCase(invoice, context.CancellationToken);
    }

    private async Task CreateRotRutCase(InvoiceDto invoice, CancellationToken cancellationToken)
    {
        var domesticServices = invoice?.DomesticService;

        if (domesticServices is not null)
        {
            var itemsWithoutHouseholdServices = invoice.Items.Where(x => x.ProductType == ProductType.Good);
            var itemsWithHouseholdServices = invoice.Items.Where(x => x.ProductType == ProductType.Service && x.DomesticService is not null);
            var itemsWithHouseholdServices2 = invoice.Items.Where(x => x.ProductType == ProductType.Good && x.DomesticService is not null);

            var hours = itemsWithHouseholdServices.Sum(x => x.Quantity);
            var laborCost = itemsWithHouseholdServices.Sum(x => x.LineTotal.AddVat(x.VatRate));
            var materialCost = itemsWithHouseholdServices2.Sum(x => x.LineTotal.AddVat(x.VatRate));

            decimal maxDeductibleAmount = 0;

            if(domesticServices.Kind == DomesticServiceKind.HomeRepairAndMaintenanceServiceType)
            {
                maxDeductibleAmount = laborCost.GetRot();
            }
            else if(domesticServices.Kind == DomesticServiceKind.HouseholdService)
            {
                maxDeductibleAmount = laborCost.GetRut();
            }

            decimal requestedAmount = domesticServices.RequestedAmount;

            DateTime paymentDate = DateTime.Now;
            decimal paidAmount = invoice.Total;
            decimal otherCosts = itemsWithoutHouseholdServices.Sum(x => x.LineTotal);

            if (requestedAmount <= 0)
            {
                throw new Exception("No deductible");
            }

            if (requestedAmount > maxDeductibleAmount)
            {
                throw new Exception("Exceeds maximum deductible amount");
            }

            var rotRutCase =
                new Domain.Entities.RotRutCase(
                    (Domain.Enums.DomesticServiceKind)domesticServices.Kind, 
                    invoice.DomesticService!.Buyer, paymentDate, laborCost, 
                    paidAmount, requestedAmount, invoice.Id, otherCosts, hours, materialCost, null);

            if (domesticServices.Kind == DomesticServiceKind.HomeRepairAndMaintenanceServiceType)
            {
                var first = itemsWithHouseholdServices.First();

                rotRutCase.Rot = new Domain.Entities.Rot() {
                    ServiceType = (Domain.Enums.HomeRepairAndMaintenanceServiceType?)first.DomesticService!.HomeRepairAndMaintenanceServiceType,
                    PropertyDesignation =  invoice.DomesticService!.PropertyDetails!.PropertyDesignation,
                    ApartmentNo =  invoice.DomesticService!.PropertyDetails!.ApartmentNo,
                    OrganizationNo =  invoice.DomesticService!.PropertyDetails!.OrganizationNo
                };
            }
            else if (domesticServices.Kind == DomesticServiceKind.HouseholdService)
            {
                var first = itemsWithHouseholdServices.First();

                rotRutCase.Rut = new Domain.Entities.Rut() {
                    ServiceType = (Domain.Enums.HouseholdServiceType?)first.DomesticService!.HouseholdServiceType
                };
            }

            _context.RotRutCases.Add(rotRutCase);

            await _context.SaveChangesAsync(cancellationToken);
        }
    }

    /*
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
    */
}