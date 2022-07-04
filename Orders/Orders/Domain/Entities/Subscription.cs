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

    interface ISubscriptionParameters
    {
        bool AutoRenew { get; set; }
        Recurrence Recurrence { get; set; }
        int? EveryDays { get; set; }
        int? EveryWeeks { get; set; }
        WeekDays? OnWeekDays { get; set; }
        int? EveryMonths { get; set; }
        int? EveryYears { get; set; }
        int? OnDay { get; set; }
        DayOfWeek? OnDayOfWeek { get; set; }
        Month? InMonth { get; set; }
        TimeSpan StartTime { get; set; }
        TimeSpan? Duration { get; set; }
    }

    public class SubscriptionPlan : AuditableEntity, ISoftDelete, ISubscriptionParameters
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = null!;

        public string? Description { get; set; }

        public int? CustomerId { get; set; } // This Subscription belongs to a Customer

        public string? ItemId { get; set; } // This Subscription belongs to a Product

        public decimal? Price { get; set; }

        //public CurrencyAmount? Price { get; set; }

        public bool AutoRenew { get; set; }

        public Recurrence Recurrence { get; set; }

        //public bool RescheduleWhenOnWeekend { get; set; } = true;

        public int? EveryDays { get; set; }

        public int? EveryWeeks { get; set; }

        public WeekDays? OnWeekDays { get; set; }

        public int? EveryMonths { get; set; } // Every two months

        public int? EveryYears { get; set; }

        public int? OnDay { get; set; } // 3rd of January. If OnDayOfWeek set to i.e. Tuesday, 3rd Tuesday

        public DayOfWeek? OnDayOfWeek { get; set; }

        public Month? InMonth { get; set; }

        public TimeSpan StartTime { get; set; }

        public TimeSpan? Duration { get; set; }

        public DateTime? Deleted { get; set; }

        public string? DeletedById { get; set; }

        public SubscriptionPlan WithName(string name)
        {
            Name = name;

            return this;
        }

        public SubscriptionPlan WithDescription(string description)
        {
            Description = description;

            return this;
        }

        public SubscriptionPlan WithEndTime(TimeSpan value)
        {
            Duration = value - StartTime;

            return this;
        }
    }
}