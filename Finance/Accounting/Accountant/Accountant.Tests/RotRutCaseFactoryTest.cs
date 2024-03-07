using YourBrand.Accountant.Domain;

using YourBrand.Invoicing.Client;

using Shouldly;

namespace Accountant.Tests;

public class EntriesFactoryTest
{
    [Fact]
    public void SimpleInvoice()
    {
        InvoiceDto invoice = GetInvoice();

        var cls = new EntriesFactory();
        var entries = cls.CreateEntriesFromInvoice(invoice);

        var debit = entries.Sum(x => x.Debit.GetValueOrDefault());
        var credit = entries.Sum(x => x.Credit.GetValueOrDefault());

        (debit - credit).ShouldBe(0);
    }

    [Fact]
    public void InvoiceForRutService()
    {
        InvoiceDto invoice = GetInvoiceRut();

        var cls = new EntriesFactory();
        var entries = cls.CreateEntriesFromInvoice(invoice);

        var debit = entries.Sum(x => x.Debit.GetValueOrDefault());
        var credit = entries.Sum(x => x.Credit.GetValueOrDefault());

        (debit - credit).ShouldBe(0);
    }

    [Fact]
    public void InvoiceForRutServiceWithMaterial()
    {
        InvoiceDto invoice = GetInvoiceRut2();

        var cls = new EntriesFactory();
        var entries = cls.CreateEntriesFromInvoice(invoice);

        var debit = entries.Sum(x => x.Debit.GetValueOrDefault());
        var credit = entries.Sum(x => x.Credit.GetValueOrDefault());

        (debit - credit).ShouldBe(0);
    }

    private static InvoiceDto GetInvoice()
    {
        var invoice = new InvoiceDto();
        invoice.Items.Add(new InvoiceItemDto()
        {
            ProductType = ProductType.Service,
            Description = "Item 1",
            UnitPrice = 560,
            Unit = "pcs",
            VatRate = 0.25,
            Quantity = 2,
            LineTotal = 1120
        });
        invoice.SubTotal = 1120;
        invoice.Vat = 280;
        invoice.Total = 1400;
        return invoice;
    }


    private static InvoiceDto GetInvoiceRut()
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
        invoice.Vat = 280;
        invoice.Total = 1400 - 700;
        return invoice;
    }

    private static InvoiceDto GetInvoiceRut2()
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
        invoice.Items.Add(new InvoiceItemDto()
        {
            ProductType = ProductType.Good,
            Description = "Material",
            UnitPrice = 100,
            Unit = "pcs",
            VatRate = 0.25,
            Quantity = 1,
            LineTotal = 100,
            IsTaxDeductibleService = true
        });
        invoice.DomesticService = new InvoiceDomesticServiceDto
        {
            Kind = DomesticServiceKind.HouseholdService,
            Description = "",
            Buyer = "76660",
            RequestedAmount = 700
        };
        invoice.SubTotal = 1120 + 100;
        invoice.Vat = 280 + 20;
        invoice.Total = 1400 + 120 - 700;
        return invoice;
    }
}
