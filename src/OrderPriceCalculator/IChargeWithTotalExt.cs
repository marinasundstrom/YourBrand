namespace OrderPriceCalculator;

public static class IChargeWithTotalExt
{
    /// <summary>
    /// Update the Charge as applied to the specified Order.
    /// </summary>
    public static IChargeWithTotal Update(this IChargeWithTotal charge, IOrder order)
    {
        charge.Total = charge.Total(order);

        return charge;
    }

    /// <summary>
    /// Update the Charge as applied to the specified OrderItem.
    /// </summary>
    public static IChargeWithTotal Update(this IChargeWithTotal charge, IOrderItem orderItem)
    {
        charge.Total = charge.Total(orderItem);

        return charge;
    }

    /// <summary>
    /// Update the Charges as applied to the specified Order.
    /// </summary>
    public static void Update(this IEnumerable<IChargeWithTotal> charges, IOrder order)
    {
        foreach (var d in charges)
            d.Update(order);
    }

    /// <summary>
    /// Update the Charges as applied to the specified OrderItem.
    /// </summary>
    public static void Update(this IEnumerable<IChargeWithTotal> charges, IOrderItem orderItem)
    {
        foreach (var d in charges)
            d.Update(orderItem);
    }
}