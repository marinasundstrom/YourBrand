namespace YourBrand.Sales;

public class LastBusinessDayOfMonth
{
    public static DateTime GetLastBusinessDayOfMonth(int year, int month)
    {
        // Start from the last day of the month
        DateTime lastDayOfMonth = new DateTime(year, month, DateTime.DaysInMonth(year, month));

        // Move backward to find the last non-weekend day
        while (lastDayOfMonth.DayOfWeek == DayOfWeek.Saturday || lastDayOfMonth.DayOfWeek == DayOfWeek.Sunday)
        {
            lastDayOfMonth = lastDayOfMonth.AddDays(-1);
        }

        return lastDayOfMonth;
    }

    public static void Main()
    {
        int year = 2024;
        int month = 10; // October

        DateTime lastBusinessDay = GetLastBusinessDayOfMonth(year, month);
        Console.WriteLine($"The last business day of {year}-{month:00} is: {lastBusinessDay:yyyy-MM-dd}");
    }

    public static DateTime GetLastWeekdayOfMonth(int year, int month, DayOfWeek targetDay)
    {
        // Start from the last day of the month
        DateTime lastDayOfMonth = new DateTime(year, month, DateTime.DaysInMonth(year, month));

        // Move backward to find the last occurrence of the targetDay
        while (lastDayOfMonth.DayOfWeek != targetDay)
        {
            lastDayOfMonth = lastDayOfMonth.AddDays(-1);
        }

        return lastDayOfMonth;
    }

    public static void Main2()
    {
        int year = 2024;
        int month = 10; // October
        DayOfWeek targetDay = DayOfWeek.Friday; // Desired weekday (e.g., last Friday of the month)

        DateTime lastWeekday = GetLastWeekdayOfMonth(year, month, targetDay);
        Console.WriteLine($"The last {targetDay} of {year}-{month:00} is: {lastWeekday:yyyy-MM-dd}");
    }
}