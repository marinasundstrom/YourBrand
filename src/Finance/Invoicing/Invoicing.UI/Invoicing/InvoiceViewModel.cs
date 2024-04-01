using System.ComponentModel.DataAnnotations;

using YourBrand.Invoicing.Client;

namespace YourBrand.Invoicing.Invoicing;

public class InvoiceViewModel
{
    private readonly List<InvoiceItemViewModel> _items = new List<InvoiceItemViewModel>();
    private readonly List<InvoiceVatAmountViewModel> _vatAmounts = new List<InvoiceVatAmountViewModel>();

    public string Id { get; set; }

    public string? InvoiceNo { get; set; }

    [Required]
    public DateTime? Date { get; set; }

    [Required]
    public TimeSpan? Time { get; set; }

    [Required]
    public InvoiceStatus Status { get; set; }

    public string? Reference { get; set; }

    public string? Note { get; set; }

    public DateTime? DueDate { get; set; }


    public IEnumerable<InvoiceItemViewModel> Items => _items;

    public void AddItem(InvoiceItemViewModel item)
    {
        _items.Add(item);

        Update();
    }

    public void RemoveItem(InvoiceItemViewModel item)
    {
        _items.Remove(item);

        Update();
    }

    public List<InvoiceVatAmountViewModel> VatAmounts => _vatAmounts;

    public decimal Discount => Items.Sum(i => (decimal)i.Quantity * i.Discount.GetValueOrDefault());

    public decimal SubTotal => Items.Sum(i => i.LineTotal) - Vat;

    public decimal Vat => Items.Sum(i => i.LineTotal.GetVatFromTotal(i.VatRate));

    public decimal Total
    {
        get
        {
            var total = Items.Sum(i => i.LineTotal);
            total -= DomesticService?.RequestedAmount.GetValueOrDefault() ?? 0;
            return total;
        }
    }

    public decimal? Paid { get; set; }

    public InvoiceDomesticServiceViewModel? DomesticService { get; set; }

    public void Update()
    {
        UpdateVatAmounts();
    }

    private void UpdateVatAmounts()
    {
        VatAmounts.ForEach(x =>
        {
            x.Vat = 0;
            x.SubTotal = 0;
            x.Total = 0;
        });

        foreach (var item in Items)
        {
            var vatAmount = VatAmounts.FirstOrDefault(x => x.VatRate == item.VatRate);
            if (vatAmount is null)
            {
                vatAmount = new InvoiceVatAmountViewModel()
                {
                    VatRate = item.VatRate, //.GetValueOrDefault(),
                    Name = $"{item.VatRate * 100}%"
                };

                VatAmounts.Add(vatAmount);
            }

            vatAmount.SubTotal += item.LineTotal - item.Vat;

            if (vatAmount.Vat is null /*  && item.VatRate is not null */)
            {
                vatAmount.Vat = 0;
            }

            vatAmount.Vat += item.Vat;
            vatAmount.Total += item.LineTotal;
        }

        VatAmounts.ToList().ForEach(x =>
        {
            if (x.Vat == 0 && x.Total == 0)
            {
                VatAmounts.Remove(x);
            }
        });

        var harMoreThanOneVatRate = VatAmounts
            .Where(x => !x.IsTotal)
            .Count(x => x.GetVat() >= 0) > 1;

        Console.WriteLine("Has more than one: {0}", harMoreThanOneVatRate);

        var totalVatAmount = VatAmounts.FirstOrDefault(x => x.VatRate == null);

        if (totalVatAmount is null)
        {
            if (!harMoreThanOneVatRate)
            {
                return;
            }

            totalVatAmount = new InvoiceVatAmountViewModel()
            {
                VatRate = null,
                Name = $"Total"
            };

            _vatAmounts.Add(totalVatAmount);
        }

        VatAmounts.Sort((x, y) => x.Order.CompareTo(y.Order));

        if (!harMoreThanOneVatRate && totalVatAmount is not null)
        {
            _vatAmounts.Remove(totalVatAmount);
            return;
        }

        totalVatAmount.SubTotal = Items.Sum(x => x.SubTotal);
        totalVatAmount.Vat = Items.Sum(x => x.Vat);
        totalVatAmount.Total = Items.Sum(x => x.LineTotal);
    }
}

public sealed record InvoiceVatAmountViewModel
{
    public string Name { get; set; }
    public double? VatRate { get; set; }
    public decimal SubTotal { get; set; }
    public decimal? Vat { get; set; }
    public decimal Total { get; set; }

    public bool IsTotal => VatRate is null;

    public decimal GetVat() => Vat.GetValueOrDefault();

    public int Order
    {
        get
        {
            if (VatRate is null) return 999;

            return (int)(VatRate.GetValueOrDefault() * 100);
        }
    }
}