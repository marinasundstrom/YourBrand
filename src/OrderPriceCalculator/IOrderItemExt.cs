namespace OrderPriceCalculator;

public static class IOrderItemExt
{
    public static decimal SubTotal(this IOrderItem orderItem)
    {
        var chargeWithoutVat = orderItem
               .Charge().GetValueOrDefault()
               .SubtractVat(orderItem.VatRate);

        var discountWithoutVat = orderItem
             .Discount().GetValueOrDefault()
             .SubtractVat(orderItem.VatRate);

        return (decimal)orderItem.Quantity * orderItem.Price.SubtractVat(orderItem.VatRate) + chargeWithoutVat + discountWithoutVat;

    }

    public static decimal Vat(this IOrderItem orderItem)
    {
        var chargeVat = orderItem
               .Charge().GetValueOrDefault()
               .GetVatIncl(orderItem.VatRate);

        var discountVat = orderItem
               .Discount().GetValueOrDefault()
               .GetVatIncl(orderItem.VatRate);

        var v = (orderItem.Price * (decimal)orderItem.Quantity).GetVatIncl(orderItem.VatRate) + chargeVat + discountVat;

        return v; ;
    }

    public static decimal Total(this IOrderItem orderItem, bool withCharge = true, bool withDiscount = true)
    {
        var sum = orderItem.Price * (decimal)orderItem.Quantity;

        if (orderItem is IHasCharges oi && withCharge)
        {
            sum += oi.Charges.Sum(orderItem);
        }

        if (orderItem is IHasDiscounts oi2 && withDiscount)
        {
            sum += oi2.Discounts.Sum(orderItem);
        }

        return sum;
    }
}