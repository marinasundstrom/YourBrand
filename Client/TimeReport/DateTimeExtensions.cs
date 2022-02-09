using System;
using System.Globalization;

namespace TimeReport;

public static class DateTimeExtensions
{
    /// <summary>
    /// Gets the date on which the specified week starts.
    /// </summary>
    /// <param name="dt"></param>
    /// <param name="startOfWeek"></param>
    /// <returns></returns>
    // Stack Overflow: https://stackoverflow.com/questions/38039/how-can-i-get-the-datetime-for-the-start-of-the-week
    public static DateTime StartOfWeek(this DateTime dt, DayOfWeek startOfWeek)
    {
        int diff = (7 + (dt.DayOfWeek - startOfWeek)) % 7;
        return dt.AddDays(-1 * diff).Date;
    }

    public static int GetCurrentWeek(this DateTime dateTime)
    {
        return CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(dateTime, CalendarWeekRule.FirstFullWeek, CultureInfo.CurrentCulture.DateTimeFormat.FirstDayOfWeek);
    }

}

public static class DateTimeHelpers
{
    /// <summary>
    /// Gets the values of DayOfWeek ordered according to the current culture.
    /// </summary>
    /// <returns></returns>
    // Stack Overflow: https://stackoverflow.com/questions/18591039/orderbydayofweek-to-treat-sunday-as-the-end-of-the-week
    public static IEnumerable<DayOfWeek> GetDaysOfWeek()
    {
        // all days of week
        var daysOfWeek = Enum.GetValues(typeof(DayOfWeek)).Cast<DayOfWeek>();

        // get first day of week from current culture
        var firstDayOfWeek = CultureInfo.CurrentCulture.DateTimeFormat.FirstDayOfWeek;

        // all days of week ordered from first day of week
        return daysOfWeek.OrderBy(x => (x - firstDayOfWeek + 7) % 7);
    }

    public static IEnumerable<DateTime> GetDatesInWeek(int year, int week)
    {
        return GetDaysOfWeek().Select(dayOfWeek => ISOWeek.ToDateTime(year, week, dayOfWeek));
    }
}