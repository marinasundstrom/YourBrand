using Microsoft.Extensions.DependencyInjection;

using Polly;

using YourBrand.Accounting.Domain.Entities;
using YourBrand.Tenancy;

namespace YourBrand.Accounting.Infrastructure.Persistence;

public static class Seed
{
    public static bool Run { get; set; } = true;

    public static bool RecreateDatabase { get; set; } = true;

    public static bool SeedAccounts { get; set; } = true;

    public static bool SeedVerifications { get; set; } = true;

    public static async Task SeedAsync(this IServiceProvider serviceProvider)
    {
        if (!Run)
        {
            return;
        }

        using var scope = serviceProvider.CreateScope();

        var tenantContext = scope.ServiceProvider.GetRequiredService<ISettableTenantContext>();
        tenantContext.SetTenantId(TenantConstants.TenantId);

        using var context = scope.ServiceProvider.GetRequiredService<AccountingContext>();

        var ledgerEntryIdGenerator = scope.ServiceProvider.GetRequiredService<ILedgerEntryIdGenerator>();

        if (RecreateDatabase)
        {
            await context.Database.EnsureDeletedAsync();
            await context.Database.EnsureCreatedAsync();
        }

        if (SeedAccounts)
        {
            DoSeedAccounts(context, TenantConstants.OrganizationId);
        }

        if (SeedVerifications)
        {
            await DoSeedVerifications(context, TenantConstants.OrganizationId, ledgerEntryIdGenerator);
        }

        await context.SaveChangesAsync();
    }

    private static async void DoSeedAccounts(AccountingContext context, string organizationId)
    {
        context.Accounts.AddRange(Accounts.GetAll(organizationId));

        await context.SaveChangesAsync();
    }

    static int journalEntryId = 1;

    private static async Task DoSeedVerifications(AccountingContext context, string organizationId, ILedgerEntryIdGenerator ledgerEntryIdGenerator)
    {
        await InsertMoney(context, organizationId, ledgerEntryIdGenerator);

        await YouSendAnInvoiceToCustomer(context, organizationId, ledgerEntryIdGenerator);
        await TheCustomerPaysTheInvoice(context, organizationId, ledgerEntryIdGenerator);
        await YouReceiveAInvoice(context, organizationId, ledgerEntryIdGenerator);
        await YouTransferFromPlusGiroToCorporateAccount(context, organizationId, ledgerEntryIdGenerator);
        await YouPayForTheInvoice(context, organizationId, ledgerEntryIdGenerator);
        await YouWithdrawMoneyAsSalary(context, organizationId, ledgerEntryIdGenerator);

        //YouTransferFromTaxAccountToCorporateAccount(context, organizationId);
    }

    private static async Task InsertMoney(AccountingContext context, string organizationId, ILedgerEntryIdGenerator ledgerEntryIdGenerator)
    {
        var journalEntry = new JournalEntry(journalEntryId++,
            DateTimeOffset.UtcNow.Subtract(TimeSpan.FromDays(19)),
            "Du sätter in egna pengar på företagskontot");
        journalEntry.OrganizationId = organizationId;

        context.JournalEntries.Add(journalEntry);

        await journalEntry.AddCreditEntry(await context.GetAccount(2018), 30000m, null, ledgerEntryIdGenerator);
        await journalEntry.AddDebitEntry(await context.GetAccount(1930), 30000m, null, ledgerEntryIdGenerator);

        await context.SaveChangesAsync();
    }

    private static async Task YouSendAnInvoiceToCustomer(AccountingContext context, string organizationId, ILedgerEntryIdGenerator ledgerEntryIdGenerator)
    {
        var journalEntry = new JournalEntry(journalEntryId++, DateTimeOffset.UtcNow.Subtract(TimeSpan.FromDays(19)), "Du skickar en faktura");
        journalEntry.OrganizationId = organizationId;

        context.JournalEntries.Add(journalEntry);

        await journalEntry.AddDebitEntry(await context.GetAccount(1510), 10000m, null, ledgerEntryIdGenerator);
        await journalEntry.AddCreditEntry(await context.GetAccount(2610), 2000m, null, ledgerEntryIdGenerator);
        await journalEntry.AddCreditEntry(await context.GetAccount(3001), 8000m, null, ledgerEntryIdGenerator);

        await context.SaveChangesAsync();
    }

    private static async Task TheCustomerPaysTheInvoice(AccountingContext context, string organizationId, ILedgerEntryIdGenerator ledgerEntryIdGenerator)
    {
        var journalEntry = new JournalEntry(journalEntryId++, DateTimeOffset.UtcNow.Subtract(TimeSpan.FromDays(19)), "Kunden betalar fakturan");
        journalEntry.OrganizationId = organizationId;

        context.JournalEntries.Add(journalEntry);

        await journalEntry.AddDebitEntry(await context.GetAccount(1920), 10000m, null, ledgerEntryIdGenerator);
        await journalEntry.AddCreditEntry(await context.GetAccount(1510), 10000m, null, ledgerEntryIdGenerator);

        await context.SaveChangesAsync();
    }

    private static async Task YouReceiveAInvoice(AccountingContext context, string organizationId, ILedgerEntryIdGenerator ledgerEntryIdGenerator)
    {
        var journalEntry = new JournalEntry(journalEntryId++, DateTimeOffset.UtcNow.Subtract(TimeSpan.FromDays(19)), "Du tar emot fakturan");
        journalEntry.OrganizationId = organizationId;

        context.JournalEntries.Add(journalEntry);

        await journalEntry.AddDebitEntry(await context.GetAccount(4000), 4000m, null, ledgerEntryIdGenerator);
        await journalEntry.AddDebitEntry(await context.GetAccount(2640), 1000m, null, ledgerEntryIdGenerator);
        await journalEntry.AddCreditEntry(await context.GetAccount(2440), 5000m, null, ledgerEntryIdGenerator);

        await context.SaveChangesAsync();
    }

    private static async Task YouPayForTheInvoice(AccountingContext context, string organizationId, ILedgerEntryIdGenerator ledgerEntryIdGenerator)
    {
        var journalEntry = new JournalEntry(journalEntryId++, DateTimeOffset.UtcNow.Subtract(TimeSpan.FromDays(19)), "Du betalar fakturan");
        journalEntry.OrganizationId = organizationId;

        context.JournalEntries.Add(journalEntry);

        await journalEntry.AddDebitEntry(await context.GetAccount(2440), 5000m, null, ledgerEntryIdGenerator);
        await journalEntry.AddCreditEntry(await context.GetAccount(1930), 5000m, null, ledgerEntryIdGenerator);

        await context.SaveChangesAsync();
    }


    private static async Task YouWithdrawMoneyAsSalary(AccountingContext context, string organizationId, ILedgerEntryIdGenerator ledgerEntryIdGenerator)
    {
        var journalEntry = new JournalEntry(journalEntryId++, DateTimeOffset.UtcNow.Subtract(TimeSpan.FromDays(19)), "Du tar ut egen lön");
        journalEntry.OrganizationId = organizationId;

        context.JournalEntries.Add(journalEntry);

        await journalEntry.AddDebitEntry(await context.GetAccount(2013), 30000, null, ledgerEntryIdGenerator);
        await journalEntry.AddCreditEntry(await context.GetAccount(1930), 30000m, null, ledgerEntryIdGenerator);

        await context.SaveChangesAsync();
    }

    private static async Task YouTransferFromPlusGiroToCorporateAccount(AccountingContext context, string organizationId, ILedgerEntryIdGenerator ledgerEntryIdGenerator)
    {
        var journalEntry = new JournalEntry(journalEntryId++, DateTimeOffset.UtcNow.Subtract(TimeSpan.FromDays(19)), "Du överför pengar från PlusGiro till företagskonto");
        journalEntry.OrganizationId = organizationId;

        context.JournalEntries.Add(journalEntry);

        await journalEntry.AddCreditEntry(await context.GetAccount(1920), 10000m, null, ledgerEntryIdGenerator);
        await journalEntry.AddDebitEntry(await context.GetAccount(1930), 10000m, null, ledgerEntryIdGenerator);

        await context.SaveChangesAsync();
    }

    private static async Task YouTransferFromTaxAccountToCorporateAccount(AccountingContext context, string organizationId, ILedgerEntryIdGenerator ledgerEntryIdGenerator)
    {
        var journalEntry = new JournalEntry(journalEntryId++, DateTimeOffset.UtcNow.Subtract(TimeSpan.FromDays(19)), "Du överför pengar från skattekonto till företagskonto");
        journalEntry.OrganizationId = organizationId;

        context.JournalEntries.Add(journalEntry);

        await journalEntry.AddCreditEntry(await context.GetAccount(1630), 4000m, null, ledgerEntryIdGenerator);
        await journalEntry.AddDebitEntry(await context.GetAccount(1930), 4000m, null, ledgerEntryIdGenerator);

        await context.SaveChangesAsync();
    }
}