using System;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

using YourBrand.Orders.Domain.Entities;

namespace YourBrand.Orders;

public static class Extensions
{
    public static IQueryable<Order> IncludeAll(this IQueryable<Order> orders,
        bool includeItems = true,
        bool includeDiscounts = true,
        bool includeCharges = true,
        bool includeSubscriptions = true,
        bool includeCustomFields = true)
    {
        var query = orders
            .Include(c => c.Status)
            .Include(c => c.Totals)
            .AsQueryable();

        if (includeCharges)
        {
            query = query
                .Include(c => c.Charges);
        }

        if (includeDiscounts)
        {
            query = query
                .Include(c => c.Discounts);
        }

        if (includeSubscriptions)
        {
            query = query
                .Include(c => c.Subscription)
                .ThenInclude(c => c!.SubscriptionPlan);
        }

        if (includeCustomFields)
        {
            query = query
                .Include(c => c.CustomFields);
        }

        if (includeItems)
        {
            query = query.Include(c => c.Items);

            if (includeCharges)
            {
                query = query
                    .Include(c => c.Items)
                    .ThenInclude(c => c.Charges);
            }

            if (includeDiscounts)
            {
                query = query
                    .Include(c => c.Items)
                    .ThenInclude(c => c.Discounts);
            }

            if (includeSubscriptions)
            {
                query = query
                    .Include(c => c.Items)
                    .ThenInclude(c => c.Subscription)
                    .ThenInclude(c => c!.SubscriptionPlan);
            }

            if (includeCustomFields)
            {
                query = query
                    .Include(c => c.Items)
                    .ThenInclude(c => c.CustomFields);
            }
        }

        return query;
    }
}