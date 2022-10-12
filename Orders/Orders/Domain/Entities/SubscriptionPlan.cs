using YourBrand.Orders.Domain.Common;
using YourBrand.Orders.Domain.Enums;

namespace YourBrand.Orders.Domain.Entities
{
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