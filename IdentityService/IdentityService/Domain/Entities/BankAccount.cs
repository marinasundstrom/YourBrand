// Copyright (c) Duende Software. All rights reserved.
// See LICENSE in the project root for license information.


namespace YourBrand.IdentityService.Domain.Entities;

public class BankAccount
{
    public string Id { get; private set; } = Guid.NewGuid().ToString();

    public User User { get; set; } = null!;

    public string BIC { get; set; }

    public string ClearingNo { get; set; }
}