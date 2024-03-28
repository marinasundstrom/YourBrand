namespace YourBrand.Accounting.Application.Accounts;

public record class AccountSeries(string Name, IEnumerable<decimal> Data);