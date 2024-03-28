namespace OrderPriceCalculator;

public static class IDiscountWithTotalExt
{
    /// <summary>
    /// Update the Discount as applied to the specified Order.
    /// </summary>
    public static IDiscountWithTotal Update(this IDiscountWithTotal discount, IOrder order)
    {
        discount.Total = discount.Total(order);

        return discount;
    }

    /// <summary>
    /// Update the Discount as applied to the specified OrderItem.
    /// </summary>
    public static IDiscountWithTotal Update(this IDiscountWithTotal discount, IOrderItem orderItem)
    {
        discount.Total = discount.Total(orderItem);

        return discount;
    }

    /// <summary>
    /// Update the Discounts as applied to the specified Order.
    /// </summary>
    public static void Update(this IEnumerable<IDiscountWithTotal> discounts, IOrder order)
    {
        foreach (var d in discounts)
            d.Update(order);
    }

    /// <summary>
    /// Update the Discounts as applied to the specified OrderItem.
    /// </summary>
    public static void Update(this IEnumerable<IDiscountWithTotal> discounts, IOrderItem orderItem)
    {
        foreach (var d in discounts)
            d.Update(orderItem);
    }
}