namespace YourBrand.Sales.Subscriptions.Plans;

public class SubscriptionPlanViewModel : ISubscriptionParameters
{
    public TimeInterval Frequency { get; set; }

    public int? EveryDays { get; set; } = 1;

    public int? EveryWeeks { get; set; } = 1;

    public YourBrand.Sales.WeekDays? OnWeekDays
    {
        get
        {
            WeekDays days = 0;

            foreach (var day in OnWeekDays2 ?? Enumerable.Empty<DayOfWeek>())
            {
                var d = Enum.Parse<WeekDays>(day.ToString());

                days |= d;

                Console.WriteLine(d);
            }

            /*
            if(OnWeekDays2 is not null)
            {
                Console.WriteLine(string.Join(", ", OnWeekDays2.OrderBy(x => x).Select(x => x.ToString())));
            }

            Console.WriteLine(days); */

            return (YourBrand.Sales.WeekDays)days;
        }
    }

    public IEnumerable<DayOfWeek> OnWeekDays2 { get; set; }

    public int? EveryMonths { get; set; } = 1;

    public int? EveryYears { get; set; } = 1;

    public int? OnDay { get; set; } = 1;

    public DayOfWeek? OnDayOfWeek { get; set; } = DayOfWeek.Monday;

    public Month? InMonth { get; set; } = Month.January;

    public TimeSpan StartTime { get; set; }

    public TimeSpan? Duration { get; set; }

    public bool AutoRenew { get; set; }
}