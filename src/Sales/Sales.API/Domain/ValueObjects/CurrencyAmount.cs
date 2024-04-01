namespace YourBrand.Sales.Domain.ValueObjects;

public sealed class CurrencyAmount : ValueObject
{
    internal CurrencyAmount()
    {

    }

    public CurrencyAmount(string currency, decimal amount)
    {
        Currency = currency;
        Amount = amount;
    }

    public string Currency { get; private set; } = null!;

    public decimal Amount { get; private set; }

    public override IEnumerable<object> GetEqualityComponents()
    {
        yield return Currency;
        yield return Amount;
    }

    public static CurrencyAmount operator +(CurrencyAmount lhs, CurrencyAmount rhs)
    {
        if (lhs.Currency != rhs.Currency)
        {
            throw new InvalidOperationException("Not the same currency");
        }

        return new CurrencyAmount(lhs.Currency, lhs.Amount + rhs.Amount);
    }

    public static CurrencyAmount operator -(CurrencyAmount lhs, CurrencyAmount rhs)
    {
        if (lhs.Currency != rhs.Currency)
        {
            throw new InvalidOperationException("Not the same currency");
        }

        return new CurrencyAmount(lhs.Currency, lhs.Amount - rhs.Amount);
    }

    public static CurrencyAmount operator *(CurrencyAmount currencyAmount, decimal factor)
    {
        return new CurrencyAmount(currencyAmount.Currency, currencyAmount.Amount * factor);
    }

    public static CurrencyAmount operator /(CurrencyAmount currencyAmount, decimal divisor)
    {
        return new CurrencyAmount(currencyAmount.Currency, currencyAmount.Amount / divisor);
    }

    public static implicit operator decimal(CurrencyAmount currencyAmount)
    {
        return currencyAmount.Amount;
    }
}

public static class CurrencyExt
{
    public static CurrencyAmount Currency(this decimal amount, string currency)
    {
        return new CurrencyAmount(currency, amount);
    }

    public static CurrencyAmount Sum(this IEnumerable<CurrencyAmount> source)
    {
        CurrencyAmount? sum = null;
        foreach (var currencyAmount in source)
        {
            if (sum is null)
            {
                sum = new CurrencyAmount(currencyAmount.Currency, 0);
            }

            sum += currencyAmount;
        }
        if (sum is null)
        {
            sum = new CurrencyAmount("Unknown", 0);
        }
        return sum!;
    }
}