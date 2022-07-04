namespace OrderPriceCalculator;

public static class IDiscountExt
{
    /// <summary>
    /// Get the total amount of this Discount as applied on the specified OrderItem.
    /// </summary>
    public static decimal Total(this IDiscount discount, IOrderItem orderItem)
    {
        // Standard is to apply the Discount once for every Order Item.

        int discountQuantity = 1;

        if (discount.Quantity is null && discount.Limit is not null)
        {
            throw new InvalidOperationException("Quantity must be specified when Limit is set.");
        }

        var orderItemQuantity = orderItem.Quantity;

        if (discount.Quantity is not null)
        {
            // Apply Discount to a certain Quantity. Respecting the Limit telling how many times.

            if (orderItem.Quantity > (discount.Limit * discount.Quantity))
            {
                orderItemQuantity = discount.Limit.GetValueOrDefault() * discount.Quantity.GetValueOrDefault();
            }

            discountQuantity = (int)Math.Floor(orderItemQuantity / (double)discount.Quantity);
        }
        else
        {
            discountQuantity = (int)orderItemQuantity;
        }

        if (discount.Percent is not null)
        {
            return (decimal)discount.Percent.GetValueOrDefault() * orderItem.Total(withDiscount: false) * (decimal)discountQuantity;
        }

        return discount.Amount.GetValueOrDefault() * (decimal)discountQuantity;
    }

    /// <summary>
    /// Get the total amount of this Discount as applied on the specified Order.
    /// </summary>
    public static decimal Total(this IDiscount discount, IOrder order)
    {
        if (discount.Quantity is not null)
        {
            throw new NotSupportedException();
        }

        if (discount.Percent is not null)
        {
            var total = order.TotalCore();
            return (decimal)discount.Percent.GetValueOrDefault() * total;
        }

        return discount.Amount.GetValueOrDefault();
    }

    /// <summary>
    /// Get the sum of these Discounts as applied on the specified OrderItem.
    /// </summary>
    public static decimal Sum(this IEnumerable<IDiscount> discounts, IOrderItem orderItem)
    {
        return orderItem.Discounts.Sum(d => d.Total(orderItem));
    }

    /// <summary>   
    /// Get the sum of these Discounts as applied on the specified Order.
    /// </summary>
    public static decimal Sum(this IEnumerable<IDiscount> discounts, IOrder order)
    {
        return order.Discounts.Sum(d => d.Total(order));
    }
}