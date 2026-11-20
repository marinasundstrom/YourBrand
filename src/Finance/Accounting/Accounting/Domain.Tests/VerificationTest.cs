using System;
using System.Linq;

using Shouldly;

using Xunit;

using YourBrand.Accounting.Domain.Entities;
using YourBrand.Domain;

namespace YourBrand.Accounting.Domain.Tests;

public class SupportingDocumentTest
{
    [Fact(DisplayName = "SupportingDocument is created")]
    public void SupportingDocumentIsCreated()
    {
        OrganizationId organizationId = "test";

        var accounts = Accounts.GetAll(organizationId);

        var account1510 = accounts.First(a => a.AccountNo == 1510);
        var account2610 = accounts.First(a => a.AccountNo == 2610);
        var account3001 = accounts.First(a => a.AccountNo == 3001);

        /*
        Verification supportingDocument = new(DateTimeOffset.UtcNow, "Test", null, null);
        supportingDocument.AddDebitEntry(account1510, debit: 10000m, description: null);
        supportingDocument.AddCreditEntry(account2610, credit: 2000m, description: null);
        supportingDocument.AddCreditEntry(account3001, credit: 8000m, description: null);

        supportingDocument.Entries.Count().ShouldBe(3);
        supportingDocument.Sum.ShouldBe(0);
        supportingDocument.IsValid.ShouldBeTrue();
        */
    }
}