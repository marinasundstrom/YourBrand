using YourBrand.Invoicing.Client;
using YourBrand.RotRutService.Domain;

namespace YourBrand.RotRutService.Tests;

public class RotRutCaseFactoryTest
{
    [Fact]
    public void CreateRotRutCase()
    {
        var invoice = new Invoice();
        invoice.Items.Add(new InvoiceItem()
        {
            ProductType = ProductType.Service,
            Description = "Cleaning",
            UnitPrice = 560,
            Unit = "hours",
            VatRate = 0.25,
            Quantity = 2,
            Total = 1120,
            IsTaxDeductibleService = true,
            DomesticService = new InvoiceItemDomesticService
            {
                Kind = DomesticServiceKind.HouseholdService,
                HouseholdServiceType = HouseholdServiceType.Cleaning
            }
        });
        invoice.DomesticService = new InvoiceDomesticService
        {
            Kind = DomesticServiceKind.HouseholdService,
            Description = "",
            Buyer = "76660",
            RequestedAmount = 700
        };
        invoice.SubTotal = 1120;
        invoice.Total = 1400;

        var rotRutCaseFactory = new RotRutCaseFactory();
        var rotRutCase = rotRutCaseFactory.CreateRotRutCase(invoice);
    }
}