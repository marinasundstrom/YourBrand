using System;
using System.Linq;

using Shouldly;

using Xunit;

using YourBrand.Accounting.Domain.Entities;
using YourBrand.Domain;

namespace YourBrand.Accounting.Domain.Tests;

public class VerificationTest
{
    [Fact(DisplayName = "Verification is created")]
    public void VerificationIsCreated()
    {
        OrganizationId organizationId = "test";

        var accounts = Accounts.GetAll(organizationId);

        var account1510 = accounts.First(a => a.AccountNo == 1510);
        var account2610 = accounts.First(a => a.AccountNo == 2610);
        var account3001 = accounts.First(a => a.AccountNo == 3001);

        JournalEntry verification = new(DateTime.Now, "Test", null, null);
        verification.AddDebitEntry(account1510, debit: 10000m, description: null);
        verification.AddCreditEntry(account2610, credit: 2000m, description: null);
        verification.AddCreditEntry(account3001, credit: 8000m, description: null);

        verification.Entries.Count().ShouldBe(3);
        verification.Sum.ShouldBe(0);
        verification.IsValid.ShouldBeTrue();
    }
}