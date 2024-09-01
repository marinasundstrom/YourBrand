namespace YourBrand.Accounting.Application.Accounts;

public class AccountShortDto
{
    public int AccountNo { get; set; }

    public AccountClassDto Class { get; set; } = null!;

    public string Name { get; set; } = null!;
}