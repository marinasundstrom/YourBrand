using System.Collections.Generic;
using System.Linq;

using YourBrand.Orders.Application.Subscriptions;
using YourBrand.Orders.Contracts;
using YourBrand.Orders.Domain.Entities;

namespace YourBrand.Orders.Application.Subscriptions
{
    public static class Mappings
    {
        public static SubscriptionDto Map(Subscription subscription)
        {
            return new SubscriptionDto
            {
                Id = subscription.Id
            };
        }
    }
}