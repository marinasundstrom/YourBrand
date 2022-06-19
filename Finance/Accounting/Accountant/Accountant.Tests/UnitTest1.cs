using YourBrand.Accountant.Domain;

using YourBrand.Invoices.Client;

namespace Accountant.Tests;

public class UnitTest1
{
    [Fact]
    public void Test1()
    {
        var invoice = new InvoiceDto();
        invoice.Items.Add(new InvoiceItemDto()
        {
            ProductType = ProductType.Service,
            Description = "Cleaning",
            UnitPrice = 560,
            Unit = "hours",
            VatRate = 0.25,
            Quantity = 2,
            LineTotal = 1120,
            IsTaxDeductibleService = true,
            DomesticService = new InvoiceItemDomesticServiceDto
            {
                Kind = DomesticServiceKind.HouseholdService,
                HouseholdServiceType = HouseholdServiceType.Cleaning
            }
        });
        invoice.DomesticService = new InvoiceDomesticServiceDto
        {
            Kind = DomesticServiceKind.HouseholdService,
            Description = "",
            Buyer = "76660",
            RequestedAmount = 700
        };
        invoice.SubTotal = 1120;
        invoice.Total = 1400;

        var cls = new EntriesFactory();
        var entries = cls.GetEntries(invoice);

        var debit = entries.Sum(x => x.Debit.GetValueOrDefault());
        var credit = entries.Sum(x => x.Credit.GetValueOrDefault());
    }
}
