using YourBrand.Accounting.Client;
using YourBrand.Invoicing.Client;

namespace YourBrand.Accountant.Domain;

public class EntriesFactory
{
    public IEnumerable<CreateEntry> CreateEntriesFromInvoice(InvoiceDto invoice)
    {
        var itemsGroupedByTypeAndVatRate = invoice.Items.GroupBy(item => new { item.ProductType, item.VatRate });

        List<CreateEntry> entries = new();

        entries.Add(new CreateEntry
        {
            AccountNo = 1510,
            Description = string.Empty,
            Debit = invoice.Total
        });

        foreach (var group in itemsGroupedByTypeAndVatRate)
        {
            var productType = group.Key.ProductType;
            var vatRate = group.Key.VatRate;

            var vat = group.Sum(i => i.LineTotal.GetVatFromSubTotal(i.VatRate));
            var subTotal = group.Sum(i => i.LineTotal);
             
            entries.AddRange(new[] {
                        new CreateEntry
                        {
                            AccountNo = GetVatAccount(productType, vatRate),
                            Description = string.Empty,
                            Credit = vat
                        },
                        new CreateEntry
                        {
                            AccountNo = GetIncomeAccount(productType, vatRate),
                            Description = string.Empty,
                            Credit = subTotal
                        }
                    });
        }

        if (invoice.DomesticService?.RequestedAmount is not null)
        {
            entries.Insert(0, new CreateEntry
            {
                AccountNo = 1513,
                Description = "ROT-avdrag",
                Debit = invoice.DomesticService?.RequestedAmount
            });
        }

        return entries;
    }

    public static int GetIncomeAccount(ProductType productType, double vatRate)
    {
        switch (productType)
        {
            case ProductType.Good:
                if (vatRate == 0.25)
                {
                    return 3001;
                }
                else if (vatRate == 0.12)
                {
                    return 3002;
                }
                else if (vatRate == 0.06)
                {
                    return 3003;
                }
                break;

            case ProductType.Service:
                return 3040;

                /*
                if (vatRate == 0.25)
                {
                    return 3041;
                }
                else if (vatRate == 0.12)
                {
                    return 3042;
                }
                else if (vatRate == 0.06)
                {
                    return 3043;
                }
                break;
                */
        }

        throw new Exception();
    }

    public static int GetVatAccount(ProductType productType, double vatRate)
    {
        if (vatRate == 0.25)
        {
            return 2610; //2611
        }
        else if (vatRate == 0.12)
        {
            return 2620;//2621
        }
        else if (vatRate == 0.06)
        {
            return 2630;//2631
        }

        /*
        switch (productType)
        {
            case ProductType.Good:
                if (vatRate == 0.25)
                {
                    return 2610; //2611
                }
                else if (vatRate == 0.12)
                {
                    return 2620;//2621
                }
                else if (vatRate == 0.06)
                {
                    return 2630;//2631
                }
                break;

            case ProductType.Service:
                if (vatRate == 0.25)
                {
                    return 2610; //2611
                }
                else if (vatRate == 0.12)
                {

                }
                else if (vatRate == 0.06)
                {

                }
                break;
        }
        */

        throw new Exception();
    }
}

