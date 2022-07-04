using System;
using System.Collections.Generic;

namespace YourBrand.Orders.Contracts
{
    public class UpdateSubscriptionCommand
    {
        public UpdateSubscriptionCommand(Guid subscriptionId)
        {
            SubscriptionId = subscriptionId;
        }

        public Guid SubscriptionId { get; }
    }
}