namespace YourBrand.Analytics.Domain;

public static class Extensions
{
    public static bool AddRange<T>(this HashSet<T> source, IEnumerable<T> items)
    {
        bool allAdded = true;
        foreach (T item in items)
        {
            allAdded &= source.Add(item);
        }
        return allAdded;
    }
}