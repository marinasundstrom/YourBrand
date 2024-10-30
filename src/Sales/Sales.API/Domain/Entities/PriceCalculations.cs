namespace YourBrand.Sales.Domain.Entities;

public static class PriceCalculations
{
    public static decimal CalculateVat(decimal total, double vatRate)
    {
        return Math.Round(total * (decimal)vatRate / (1 + (decimal)vatRate), 2, MidpointRounding.ToEven);
    }
}