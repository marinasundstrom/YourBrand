// Copyright (c) Duende Software. All rights reserved.
// See LICENSE in the project root for license information.


namespace YourBrand.IdentityService.Domain.Entities;

public class BankAccount
{
    private BankAccount()
    {

    }

    public BankAccount(string bic, string clearingNo)
    {
        Id = Guid.NewGuid().ToString();
        Bic = bic;
        ClearingNo = clearingNo;
    }

    public string Id { get; private set; }

    public User User { get; set; } = null!;

    public string BIC { get; set; }

    public string ClearingNo { get; set; }
    public string Bic { get; }
}