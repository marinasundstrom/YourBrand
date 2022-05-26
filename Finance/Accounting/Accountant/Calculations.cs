namespace Accountant;

public static class Calculations 
{
    public static decimal Vat(this decimal value, double vatRate)
    {
         return value - value.SubTotal(vatRate);
    }

    public static decimal SubTotal(this decimal value, double vatRate)
    {
        return value / (1m + (decimal)vatRate);
    }
}