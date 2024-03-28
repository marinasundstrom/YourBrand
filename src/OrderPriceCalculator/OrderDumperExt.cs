namespace OrderPriceCalculator;

using static Console;

public static class OrderDumperExt
{
    public static void Dump(this IOrder2 order)
    {
        WriteLine($"{"Item",-30} {"Price",-20} {"Quantity",-12} {"Total",-12}");

        foreach (var item in order.Items)
        {
            WriteLine($"{item.Description,-30} {item.Price.ToString("c") + " (" + item.VatRate * 100 + "%)",-20} {item.Quantity + " pcs",-12} {(item.Total(withCharge: false, withDiscount: false)).ToString("c"),-12}");

            foreach (var charge in item.Charges)
            {
                WriteLine($"    {charge.Description,-15}{(charge.Percent is not null ? $" {charge.Percent * 100 + "%"}" : null),-12} {charge.Total.ToString("c"),33}");
            }

            foreach (var discount in item.Discounts)
            {
                WriteLine($"    {discount.Description,-15} {(discount.Percent is not null ? (item.VatRate * 100) + "%" : null),-12} {discount.Total.ToString("c"),33}");
            }

            WriteLine();
        }

        WriteLine();

        var totals = order.Totals();

        WriteLine($"{"VAT%",-5} {"Sub Total",-12} {"VAT",-12} {"Total",-12}");

        foreach (var f in totals)
        {
            WriteLine($"{f.VatRate * 100 + "%",-5} {f.SubTotal.ToString("c"),-12} {f.Vat.ToString("c"),-12} {f.Total.ToString("c"),-12}");
        }

        WriteLine();

        if (order.Charges.Any())
        {
            WriteLine("Charges:");

            foreach (var charge in order.Charges)
            {
                WriteLine($"{charge.Description,-15}{(charge.Percent is not null ? $" {charge.Percent * 100 + "%"}" : null),-12} {charge.Total.ToString("c"),33}");
            }

            WriteLine();
        }

        if (order.Discounts.Any())
        {
            WriteLine("Discounts:");

            foreach (var discount in order.Discounts)
            {
                WriteLine($"{discount.Description,-15} {discount.Total.ToString("c"),33}");
            }

            WriteLine();
        }

        WriteLine($"Charges: {order.Charge?.ToString("c")}");
        WriteLine($"Discount: {order.Discount?.ToString("c")}");
        WriteLine($"Vat: {order.Vat().ToString("c")}");
        WriteLine($"Rounding: {order.Rounding?.ToString("c")} ");
        WriteLine($"Total: {order.Total().ToString("c")}");
    }
}