using YourBrand.Sales.Domain.Enums;

namespace YourBrand.Sales.Domain.Entities;

public class SubscriptionSchedule : IEquatable<SubscriptionSchedule>, ISubscriptionScheduleParameters
{
    public TimeInterval Frequency { get; set; }
    public int? EveryDays { get; set; }
    public int? EveryWeeks { get; set; }
    public WeekDays? OnWeekDays { get; set; }
    public int? EveryMonths { get; set; }
    public int? EveryYears { get; set; }
    public int? OnDay { get; set; }
    public DayOfWeek? OnDayOfWeek { get; set; }
    public Month? InMonth { get; set; }
    public TimeOnly? StartTime { get; set; }
    public TimeOnly? EndTime { get; set; }
    public TimeSpan? Duration { get; set; }

    // Factory method for a daily schedule
    public static SubscriptionSchedule Daily(int everyDays)
    {
        return new SubscriptionSchedule
        {
            Frequency = TimeInterval.Daily,
            EveryDays = everyDays
        };
    }

    // Factory method for a weekly schedule
    public static SubscriptionSchedule Weekly(int everyWeeks, WeekDays? onWeekDays = null)
    {
        return new SubscriptionSchedule
        {
            Frequency = TimeInterval.Weekly,
            EveryWeeks = everyWeeks,
            OnWeekDays = onWeekDays
        };
    }

    // Factory method for a monthly schedule
    public static SubscriptionSchedule Monthly(int everyMonths, int? onDay = null)
    {
        return new SubscriptionSchedule
        {
            Frequency = TimeInterval.Monthly,
            EveryMonths = everyMonths,
            OnDay = onDay,
        };
    }

    public static SubscriptionSchedule Monthly(int everyMonths, int? onDay = null, DayOfWeek? onDayOfWeek = null)
    {
        return new SubscriptionSchedule
        {
            Frequency = TimeInterval.Monthly,
            EveryMonths = everyMonths,
            OnDay = onDay,
            OnDayOfWeek = onDayOfWeek
        };
    }

    // Factory method for a quarterly schedule
    public static SubscriptionSchedule Quarterly(int? onDay = null, DayOfWeek? onDayOfWeek = null)
    {
        return new SubscriptionSchedule
        {
            Frequency = TimeInterval.Quarterly,
            OnDay = onDay,
            OnDayOfWeek = onDayOfWeek
        };
    }

    // Factory method for a yearly schedule
    public static SubscriptionSchedule Yearly(int everyYears, Month? inMonth = null, int? onDay = null, DayOfWeek? onDayOfWeek = null)
    {
        return new SubscriptionSchedule
        {
            Frequency = TimeInterval.Yearly,
            EveryYears = everyYears,
            InMonth = inMonth,
            OnDay = onDay,
            OnDayOfWeek = onDayOfWeek,
        };
    }

    public SubscriptionSchedule WithStartTime(TimeOnly? startTime)
    {
        StartTime = startTime;
        return this;
    }

    public SubscriptionSchedule WithEndTime(TimeOnly? endTime)
    {
        EndTime = endTime;
        return this;
    }

    public SubscriptionSchedule WithDuration(TimeSpan? timeSpan)
    {
        Duration = timeSpan;
        return this;
    }

    public SubscriptionSchedule Clone()
    {
        return new SubscriptionSchedule
        {
            Frequency = this.Frequency,
            EveryDays = this.EveryDays,
            EveryWeeks = this.EveryWeeks,
            OnWeekDays = this.OnWeekDays,
            EveryMonths = this.EveryMonths,
            EveryYears = this.EveryYears,
            OnDay = this.OnDay,
            OnDayOfWeek = this.OnDayOfWeek,
            InMonth = this.InMonth,
            StartTime = this.StartTime,
            EndTime = this.EndTime,
            Duration = this.Duration
        };
    }

    public void CopyTo(SubscriptionSchedule targetSchedule)
    {
        targetSchedule.Frequency = this.Frequency;
        targetSchedule.EveryDays = this.EveryDays;
        targetSchedule.EveryWeeks = this.EveryWeeks;
        targetSchedule.OnWeekDays = this.OnWeekDays;
        targetSchedule.EveryMonths = this.EveryMonths;
        targetSchedule.EveryYears = this.EveryYears;
        targetSchedule.OnDay = this.OnDay;
        targetSchedule.OnDayOfWeek = this.OnDayOfWeek;
        targetSchedule.InMonth = this.InMonth;
        targetSchedule.StartTime = this.StartTime;
        targetSchedule.EndTime = this.EndTime;
        targetSchedule.Duration = this.Duration;
    }

    public bool Equals(SubscriptionSchedule? other)
    {
        if (other is null) return false;

        return Frequency == other.Frequency &&
               EveryDays == other.EveryDays &&
               EveryWeeks == other.EveryWeeks &&
               OnWeekDays == other.OnWeekDays &&
               EveryMonths == other.EveryMonths &&
               EveryYears == other.EveryYears &&
               OnDay == other.OnDay &&
               OnDayOfWeek == other.OnDayOfWeek &&
               InMonth == other.InMonth &&
               StartTime == other.StartTime &&
               EndTime == other.EndTime &&
               Duration == other.Duration;
    }

    public override bool Equals(object? obj)
    {
        if (obj is SubscriptionSchedule other)
        {
            return Equals(other);
        }
        return false;
    }

    public override int GetHashCode()
    {
        unchecked
        {
            int hash = 17;
            hash = hash * 23 + Frequency.GetHashCode();
            hash = hash * 23 + (EveryDays?.GetHashCode() ?? 0);
            hash = hash * 23 + (EveryWeeks?.GetHashCode() ?? 0);
            hash = hash * 23 + (OnWeekDays?.GetHashCode() ?? 0);
            hash = hash * 23 + (EveryMonths?.GetHashCode() ?? 0);
            hash = hash * 23 + (EveryYears?.GetHashCode() ?? 0);
            hash = hash * 23 + (OnDay?.GetHashCode() ?? 0);
            hash = hash * 23 + (OnDayOfWeek?.GetHashCode() ?? 0);
            hash = hash * 23 + (InMonth?.GetHashCode() ?? 0);
            hash = hash * 23 + (StartTime?.GetHashCode() ?? 0);
            hash = hash * 23 + (EndTime?.GetHashCode() ?? 0);
            hash = hash * 23 + (Duration?.GetHashCode() ?? 0);
            return hash;
        }
    }
}