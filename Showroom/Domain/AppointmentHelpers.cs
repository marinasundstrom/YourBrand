public static class AppointmentHelpers 
{
    public static IEnumerable<(DateTime Start, DateTime? End)[]> Overlappings(this IEnumerable<(DateTime Start, DateTime? End)> meetings)
    {
        var last = ((DateTime Start, DateTime? End)?)null;
        foreach (var meeting in meetings.OrderBy(m => m.Start))
        {
            if (last != null && meeting.OverlapsWith(last.GetValueOrDefault()))
            {
                yield return new (DateTime Start, DateTime? End)[] { last.GetValueOrDefault(), meeting };
            }
            last = meeting;
        }
    }

    public static bool OverlapsWith(this (DateTime Start, DateTime? End) x, (DateTime Start, DateTime? End) y) 
    {
        return x.Start < y.Start ? x.End > y.Start : y.End > x.Start;
    }
}