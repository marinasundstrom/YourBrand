using System.Globalization;

namespace YourBrand;

public static class DecimalExtension
{
    private static readonly Dictionary<string, CultureInfo> ISOCurrenciesToACultureMap =
        CultureInfo.GetCultures(CultureTypes.SpecificCultures)
            .Select(c => new { c, new RegionInfo(c.Name).ISOCurrencySymbol })
            .GroupBy(x => x.ISOCurrencySymbol)
            .ToDictionary(g => g.Key, g => g.First().c, StringComparer.OrdinalIgnoreCase);

    public static string FormatCurrency(this decimal amount, string currencyCode)
    {
        CultureInfo? culture;
        if (ISOCurrenciesToACultureMap.TryGetValue(currencyCode, out culture))
            return string.Format(culture, "{0:C}", amount);
        return amount.ToString("0.00");
    }
}