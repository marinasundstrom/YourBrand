using System;

namespace Accounting.Application.Accounts;

public class AccountDto
{
    public int AccountNo { get; set; }

    public AccountClassDto Class { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string Description { get; set; } = null!;

    public decimal Balance { get; set; }
}