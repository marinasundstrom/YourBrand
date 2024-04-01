using Shouldly;

using YourBrand.Accountant.Domain;
using YourBrand.Invoicing.Client;

namespace Accountant.Tests;

public class EntriesFactoryTest
{
    [Fact]
    public void SimpleInvoice()
    {
        Invoice invoice = GetInvoice();

        var cls = new EntriesFactory();
        var entries = cls.CreateEntriesFromInvoice(invoice);

        var debit = entries.Sum(x => x.Debit.GetValueOrDefault());
        var credit = entries.Sum(x => x.Credit.GetValueOrDefault());

        (debit - credit).ShouldBe(0);
    }

    [Fact]
    public void InvoiceForRutService()
    {
        Invoice invoice = GetInvoiceRut();

        var cls = new EntriesFactory();
        var entries = cls.CreateEntriesFromInvoice(invoice);

        var debit = entries.Sum(x => x.Debit.GetValueOrDefault());
        var credit = entries.Sum(x => x.Credit.GetValueOrDefault());

        (debit - credit).ShouldBe(0);
    }

    [Fact]
    public void InvoiceForRutServiceWithMaterial()
    {
        Invoice invoice = GetInvoiceRut2();

        var cls = new EntriesFactory();
        var entries = cls.CreateEntriesFromInvoice(invoice);

        var debit = entries.Sum(x => x.Debit.GetValueOrDefault());
        var credit = entries.Sum(x => x.Credit.GetValueOrDefault());

        (debit - credit).ShouldBe(0);
    }

    private static Invoice GetInvoice()
    {
        var invoice = new Invoice();
        invoice.Items.Add(new InvoiceItem()
        {
            ProductType = ProductType.Service,
            Description = "Item 1",
            UnitPrice = 560,
            Unit = "pcs",
            VatRate = 0.25,
            Quantity = 2,
            Total = 1120
        });
        invoice.SubTotal = 1120;
        invoice.Vat = 280;
        invoice.Total = 1400;
        return invoice;
    }


    private static Invoice GetInvoiceRut()
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
        invoice.Vat = 280;
        invoice.Total = 1400 - 700;
        return invoice;
    }

    private static Invoice GetInvoiceRut2()
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
        invoice.Items.Add(new InvoiceItem()
        {
            ProductType = ProductType.Good,
            Description = "Material",
            UnitPrice = 100,
            Unit = "pcs",
            VatRate = 0.25,
            Quantity = 1,
            Total = 100,
            IsTaxDeductibleService = true
        });
        invoice.DomesticService = new InvoiceDomesticService
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