namespace YourBrand.Orders.Models;

using YourBrand.Orders.Hypermedia;

using YourBrand.Orders.Infrastructure.Persistence;

public static partial class Mapper
{
    public static YourBrand.Orders.Models.Orders Map(IEnumerable<Domain.Entities.Order> orders, string[] embed)
    {
        var r = new YourBrand.Orders.Models.Orders()
        {
            Embedded = new YourBrand.Orders.Models.OrdersEmbedded
            {
                Orders = orders.Select(i =>
                    YourBrand.Orders.Models.Mapper.Map(i, embed))
            },
            Links = new Dictionary<string, Link>()
        };

        return r;
    }

    public static YourBrand.Orders.Models.Order Map(Domain.Entities.Order order, string[] embed)
    {
        var r = new YourBrand.Orders.Models.Order()
        {
            Id = order.Id,
            OrderNo = order.OrderNo,
            Type = (OrderType)order.Type,
            Date = order.OrderDate.GetValueOrDefault(),
            Status = order.StatusId,

            SubTotal = order.SubTotal,

            Vat = order.Vat,
            VatRate = order.VatRate,

            Discount = order.Discount,
            Charge = order.Charge,

            Rounding = order.Rounding,
            Total = order.Total,

            Totals = order.Totals.Any() ? order.Totals.ToDictionary(x => x.VatRate.ToString(), x => new OrderTotal()
            {
                SubTotal = x.SubTotal,
                Vat = x.Vat,
                Total = x.Total
            }) : null,

            /*

            Payment = order.Payment is not null ? new YourBrand.Orders.Contracts.PaymentDetailsDto
            {
                Paid = order.Payment.Paid,
                Returned = order.Payment.Returned
            } : null,

            Signature = order.Signature,

            */

            CustomFields = order.CustomFields.Any() ? order.CustomFields.ToDictionary(x => x.CustomFieldId, x => (object)ConvertValue(x.Value)) : null,

            Embedded = embed.Length > 0 ? new YourBrand.Orders.Models.OrderEmbedded
            {
                Items = embed.Length > 0 && embed.Any(x => x == "items") ? order.Items.Select(i =>
                    YourBrand.Orders.Models.Mapper.Map(i, embed)) : null,
                Discounts = embed.Length > 0 && embed.Any(x => x == "discounts") ? order.Discounts.Select(i =>
                    YourBrand.Orders.Models.Mapper.Map(i, Array.Empty<string>())) : null,
                Charges = embed.Length > 0 && embed.Any(x => x == "charges") ? order.Charges.Select(i =>
                    YourBrand.Orders.Models.Mapper.Map(i, Array.Empty<string>())) : null
            } : null,
            Links = new Dictionary<string, Link> {
                { "self", new Link { Href = $"/api/orders/{order.OrderNo}" } },
            }
        };

        r.Links.Add("invoice", new Link
        {
            Href = $"/api/invoices/{order.OrderNo}",
            Title = "Get the invoice"
        });

        if (order.StatusId != "placed")
        {
            r.Links.Add("place", new Link
            {
                Href = $"/api/orders/{order.OrderNo}/place",
                Method = "post",
                Title = "Place order"
            });
        }

        return r;
    }

    public static YourBrand.Orders.Models.OrderItem Map(Domain.Entities.OrderItem i, string[] embed)
    {
        return new YourBrand.Orders.Models.OrderItem()
        {
            Id = i.Id,
            ItemId = i.ItemId,
            Description = i.Description,
            Unit = i.Unit,
            Price = i.Price,
            VatRate = i.VatRate,
            Quantity = i.Quantity,
            Charge = i.Charge,
            Discount = i.Discount,
            Total = i.Total,

            CustomFields = i.CustomFields.Any() ? i.CustomFields.ToDictionary(x => x.CustomFieldId, x => (object)ConvertValue(x.Value)) : null,

            Embedded = embed.Length > 0 ? new YourBrand.Orders.Models.OrderItemEmbedded
            {
                Discounts = embed.Length > 0 && embed.Any(x => x == "discounts") ? i.Discounts.Select(i =>
                    YourBrand.Orders.Models.Mapper.Map(i, Array.Empty<string>())) : null,
                Charges = embed.Length > 0 && embed.Any(x => x == "charges") ? i.Charges.Select(i =>
                    YourBrand.Orders.Models.Mapper.Map(i, Array.Empty<string>())) : null,
                CustomFields = embed.Length > 0 && embed.Any(x => x == "customFields") ? i.CustomFields.Select(i =>
                    YourBrand.Orders.Models.Mapper.Map(i, Array.Empty<string>())) : null
            } : null,
            Links = new Dictionary<string, Link> {
                { "self", new Link { Href = $"/api/orders/{i.Order!.OrderNo}/items/{i.Id}" } }
            }
        };
    }

    static object ConvertValue(string v)
    {
        if (bool.TryParse(v, out var boolValue))
        {
            return boolValue;
        }

        if (double.TryParse(v, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var doubleValue))
        {
            return doubleValue;
        }

        if (int.TryParse(v, out var intValue))
        {
            return intValue;
        }

        return v;
    }

    public static YourBrand.Orders.Models.OrderDiscount Map(Domain.Entities.OrderDiscount i, string[] embed)
    {
        var c = new YourBrand.Orders.Models.OrderDiscount()
        {
            Id = i.Id,
            Description = i.Description,
            DiscountId = i.DiscountId,
            Amount = i.Amount,
            Percent = i.Percent,
            Quantity = i.Quantity,
            Total = i.Total
        };
        c.Links.Add("self", new Link
        {
            Href = i.Order != null
                ? $"/api/orders/{i.Order!.OrderNo}/discounts/{i.Id}"
                : $"/api/orders/{i.OrderItem!.Order.OrderNo}/items/{i.OrderItem.Id}/discounts/{i.Id}"
        });

        return c;
    }

    public static YourBrand.Orders.Models.OrderCharge Map(Domain.Entities.OrderCharge i, string[] embed)
    {
        var d = new YourBrand.Orders.Models.OrderCharge()
        {
            Id = i.Id,
            Description = i.Description,
            ChargeId = i.ChargeId,
            Amount = i.Amount,
            Percent = i.Percent,
            Quantity = i.Quantity,
            Total = i.Total
        };

        d.Links.Add("self", new Link
        {
            Href = i.Order != null
                ? $"/api/orders/{i.Order!.OrderNo}/charges/{i.Id}"
                : $"/api/orders/{i.OrderItem!.Order.OrderNo}/items/{i.OrderItem.Id}/charges/{i.Id}"
        });

        return d;
    }

    public static YourBrand.Orders.Models.CustomField Map(Domain.Entities.CustomField i, string[] embed)
    {
        var d = new YourBrand.Orders.Models.CustomField()
        {
            Name = i.CustomFieldId,
            Value = i.Value
        };

        /*
        d.Links.Add("self", new Link
        {
            Href = i.Order != null
                ? $"/api/orders/{i.Order!.OrderNo}/charges/{i.Id}"
                : $"/api/orders/{i.OrderItem!.order.OrderNo}/items/{i.OrderItem.Id}/charges/{i.Id}"
        });
        */

        return d;
    }
}