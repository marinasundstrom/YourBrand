namespace YourBrand.Showroom;

public static class GroupingExtensions
{
    public static void Deconstruct<TKey, TValue>(this IGrouping<TKey, TValue> source, out TKey key, out IEnumerable<TValue> items)
    {
        key = source.Key;
        items = source.AsEnumerable();
    }
}