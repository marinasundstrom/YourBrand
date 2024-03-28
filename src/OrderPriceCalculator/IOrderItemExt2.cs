namespace OrderPriceCalculator;

public static class IOrderItemExt2
{
    public static IOrderItem2 Update(this IOrderItem2 orderItem)
    {
        orderItem.Vat = orderItem.Vat();
        orderItem.Total = orderItem.Total();

        if (orderItem is IHasCharges oi)
        {
            if (orderItem is IHasChargesWithTotal oi2)
            {
                oi2.Charges.Update(orderItem);
            }

            if (oi.Charges.Any())
            {
                orderItem.Charge = oi.Charges.Sum(orderItem);
            }
        }

        if (orderItem is IHasDiscounts oi3)
        {
            if (orderItem is IHasDiscountsWithTotal oi2)
            {
                oi2.Discounts.Update(orderItem);
            }

            if (oi3.Discounts.Any())
            {
                orderItem.Discount = oi3.Discounts.Sum(orderItem);
            }
        }

        return orderItem;
    }
}