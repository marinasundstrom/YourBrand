using YourBrand.Accounting.Client;
using YourBrand.Invoices.Client;

namespace YourBrand.Accountant.Domain;

public class EntriesFactory
{
    public IEnumerable<CreateEntry> GetEntries(InvoiceDto invoice)
    {
        var itemsGroupedByTypeAndVatRate = invoice.Items.GroupBy(item => new { item.ProductType, item.VatRate });

        List<CreateEntry> entries = new();

        foreach (var group in itemsGroupedByTypeAndVatRate)
        {
            var productType = group.Key.ProductType;
            var vatRate = group.Key.VatRate;

            var vat = group.Sum(i => i.LineTotal.GetVatFromSubTotal(i.VatRate));
            var subTotal = group.Sum(i => i.LineTotal);

            entries.Add(new CreateEntry
            {
                AccountNo = 1510,
                Description = string.Empty,
                Debit = invoice.Total
            });

            if (productType == ProductType.Good)
            {
                if (vatRate == 0.25)
                {
                    entries.AddRange(new[] {
                        new CreateEntry
                        {
                            AccountNo = 2610,
                            Description = string.Empty,
                            Credit = vat
                        },
                        new CreateEntry
                        {
                            AccountNo = 3001,
                            Description = string.Empty,
                            Credit = subTotal
                        }
                    });
                }
                else if (vatRate == 0.12)
                {
                    entries.AddRange(new[] {
                        new CreateEntry
                        {
                            AccountNo = 2610,
                            Description = string.Empty,
                            Credit = vat
                        },
                        new CreateEntry
                        {
                            AccountNo = 3002,
                            Description = string.Empty,
                            Credit = subTotal
                        }
                    });
                }
                else if (vatRate == 0.06)
                {
                    entries.AddRange(new[] {
                        new CreateEntry
                        {
                            AccountNo = 2610,
                            Description = string.Empty,
                            Credit = vat
                        },
                        new CreateEntry
                        {
                            AccountNo = 3003,
                            Description = string.Empty,
                            Credit = subTotal
                        }
                    });
                }
            }
            else if (productType == ProductType.Service)
            {
                if (vatRate == 0.25)
                {
                    entries.AddRange(new[] {
                        new CreateEntry
                        {
                            AccountNo = 2610,
                            Description = string.Empty,
                            Credit = vat
                        },
                        new CreateEntry
                        {
                            AccountNo = 3041,
                            Description = string.Empty,
                            Credit = subTotal
                        }
                    });
                }
            }
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
}

