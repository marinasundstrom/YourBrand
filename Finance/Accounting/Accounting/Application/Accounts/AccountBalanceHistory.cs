namespace Accounting.Application.Accounts;

public record class AccountBalanceHistory(string[] Labels, IEnumerable<AccountSeries> Series);