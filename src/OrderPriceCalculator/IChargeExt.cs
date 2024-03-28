namespace OrderPriceCalculator;

public static class IChargeExt
{
    /// <summary>
    /// Get the total amount of this Charge as applied on the specified OrderItem.
    /// </summary>
    public static decimal Total(this ICharge charge, IOrderItem orderItem)
    {
        // Standard is to apply the Charge once for every Order Item.

        int chargeQuantity = 1;

        if (charge.Quantity is null && charge.Limit is not null)
        {
            throw new InvalidOperationException("Quantity must be specified when Limit is set.");
        }

        var orderItemQuantity = orderItem.Quantity;

        if (charge.Quantity is not null)
        {
            // Apply Charge to a certain Quantity. Respecting the Limit telling how many times.


            if (orderItem.Quantity > (charge.Limit * charge.Quantity))
            {
                orderItemQuantity = charge.Limit.GetValueOrDefault() * charge.Quantity.GetValueOrDefault();
            }

            chargeQuantity = (int)Math.Floor(orderItemQuantity / (double)charge.Quantity);
        }
        else
        {
            chargeQuantity = (int)orderItemQuantity;
        }

        if (charge.Percent is not null)
        {
            return (decimal)charge.Percent.GetValueOrDefault() * orderItem.Total(withDiscount: false, withCharge: false) * (decimal)chargeQuantity;
        }

        return charge.Amount.GetValueOrDefault() * (decimal)chargeQuantity;
    }

    /// <summary>
    /// Get the total amount of this Charge as applied on the specified Order.
    /// </summary>
    public static decimal Total(this ICharge charge, IOrder order)
    {
        if (charge.Quantity is not null)
        {
            throw new NotSupportedException();
        }

        if (charge.Percent is not null)
        {
            var total = order.TotalCore();
            return (decimal)charge.Percent.GetValueOrDefault() * total;
        }

        return charge.Amount.GetValueOrDefault();
    }

    /// <summary>
    /// Get the sum of these Charges as applied on the specified OrderItem.
    /// </summary>
    public static decimal Sum(this IEnumerable<ICharge> charges, IOrderItem orderItem)
    {
        return orderItem.Charges.Sum(d => d.Total(orderItem));
    }

    /// <summary>   
    /// Get the sum of these Charges as applied on the specified Order.
    /// </summary>
    public static decimal Sum(this IEnumerable<ICharge> charges, IOrder order)
    {
        return order.Charges.Sum(d => d.Total(order));
    }
}