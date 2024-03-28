namespace OrderPriceCalculator;

public static class VatExt
{
    /// <summary>
    /// Add VAT.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="vatRate"></param>
    /// <returns></returns>
    public static decimal AddVat(this decimal value, double vatRate)
    {
        return value * (1 + (decimal)vatRate);
    }

    /// <summary>
    /// Subtract VAT.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="vatRate"></param>
    /// <returns></returns>
    public static decimal SubtractVat(this decimal value, double vatRate)
    {
        var vatIncluded = value.GetVatIncl(vatRate);

        return value - vatIncluded;
    }

    /// <summary>
    /// Get the VAT that is included.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="vatRate"></param>
    /// <returns></returns>
    public static decimal GetVatIncl(this decimal value, double vatRate)
    {
        return value - (value / (1m + (decimal)vatRate));
    }

    /// <summary>
    /// Get VAT for price with specified rate.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="vatRate"></param>
    /// <returns></returns>
    public static decimal Vat(this decimal value, double vatRate)
    {
        return value * (decimal)vatRate;
    }
}