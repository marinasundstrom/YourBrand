namespace OrderPriceCalculator;

public static class IHasChargesExt
{
    /// <summary>
    /// Gets the total amount of Charges.
    /// </summary>
    public static decimal? Charge(this IHasCharges hasCharges)
    {
        decimal? charge = null;

        if (hasCharges is IOrder order)
        {
            foreach (var item in order.Items)
            {
                if (!item.Charges.Any())
                    continue;

                if (charge is null) charge = 0;

                charge += item.Charge();
            }

            if (order.Charges.Any())
            {
                charge += order.Charges.Sum(order);
            }
        }
        else if (hasCharges is IOrderItem orderItem)
        {
            if (orderItem.Charges.Any())
            {
                if (charge is null) charge = 0;

                return orderItem.Charges.Sum(orderItem);
            }
        }

        return charge;
    }
}