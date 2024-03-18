namespace YourBrand.Ticketing;

public static class Calculations
{
    public static decimal AddVat(this decimal subTotal, double vatRate)
    {
        return subTotal * (decimal)(1 + vatRate);
    }

    public static decimal GetVatFromSubTotal(this decimal subTotal, double vatRate)
    {
        return subTotal * (decimal)vatRate;
    }

    public static decimal GetVatFromTotal(this decimal total, double vatRate)
    {
        return total - total.GetSubTotal(vatRate);
    }

    public static decimal GetSubTotal(this decimal total, double vatRate)
    {
        return total / (1m + (decimal)vatRate);
    }

    public static decimal ApplyRot(this decimal total)
    {
        return total * (1 - 0.3m);
    }

    public static decimal ApplyRut(this decimal total)
    {
        return total * 0.5m;
    }

    public static decimal GetRot(this decimal total)
    {
        return total * 0.3m;
    }

    public static decimal GetRut(this decimal total)
    {
        return total * 0.5m;
    }
}