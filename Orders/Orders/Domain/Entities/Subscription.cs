using System;
using System.Collections.Generic;

using Newtonsoft.Json;

using YourBrand.Orders.Contracts;
using YourBrand.Orders.Domain.Common;
using YourBrand.Orders.Domain.Enums;

namespace YourBrand.Orders.Domain.Entities
{
    public class Subscription : AuditableEntity, ISoftDelete, ISubscriptionParameters
    {
        public Guid Id { get; set; }

        public int? CustomerId { get; set; }

        public Order? Order { get; set; }

        public Guid? OrderId { get; set; }

        public OrderItem? OrderItem { get; set; }

        public Guid? OrderItemId { get; set; }

        public SubscriptionPlan? SubscriptionPlan { get; set; }

        public Guid? SubscriptionPlanId { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public SubscriptionStatus Status { get; set; }

        public DateTime StatusDate { get; set; }

        public string? Note { get; set; }

        public List<Order> Orders { get; } = new List<Order>();

        public List<OrderItem> OrderItems { get; } = new List<OrderItem>();

        public DateTime? Deleted { get; set; }

        public string? DeletedById { get; set; }

        public Recurrence Recurrence { get; set; }
        public int? EveryDays { get; set; }
        public int? EveryWeeks { get; set; }
        public WeekDays? OnWeekDays { get; set; }
        public int? EveryMonths { get; set; }
        public int? EveryYears { get; set; }
        public int? OnDay { get; set; }
        public DayOfWeek? OnDayOfWeek { get; set; }
        public Month? InMonth { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan? Duration { get; set; }
        public bool AutoRenew { get; set; }
    }
}