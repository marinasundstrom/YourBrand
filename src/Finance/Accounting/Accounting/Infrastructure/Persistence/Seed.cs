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

    static readonly int verificationId = 1;

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
            await DoSeedVerifications(context, TenantConstants.OrganizationId);
        }

        await context.SaveChangesAsync();
    }

    private static async void DoSeedAccounts(AccountingContext context, string organizationId)
    {
        context.Accounts.AddRange(Accounts.GetAll(organizationId));

        await context.SaveChangesAsync();
    }

    private static async Task DoSeedVerifications(AccountingContext context, string organizationId)
    {
        await InsertMoney(context, organizationId);

        await YouSendAnInvoiceToCustomer(context, organizationId);
        await TheCustomerPaysTheInvoice(context, organizationId);
        await YouReceiveAInvoice(context, organizationId);
        await YouTransferFromPlusGiroToCorporateAccount(context, organizationId);
        await YouPayForTheInvoice(context, organizationId);
        await YouWithdrawMoneyAsSalary(context, organizationId);

        //YouTransferFromTaxAccountToCorporateAccount(context, organizationId);
    }

    private static async Task InsertMoney(AccountingContext context, string organizationId)
    {
        var verification = new JournalEntry(
            DateTime.Now.Subtract(TimeSpan.FromDays(19)),
            "Du sätter in egna pengar på företagskontot");
        verification.OrganizationId = organizationId;

        context.JournalEntries.Add(verification);

        verification.AddCreditEntry(await context.GetAccount(2018), 30000m);
        verification.AddDebitEntry(await context.GetAccount(1930), 30000m);

        context.SaveChanges();
    }

    private static async Task YouSendAnInvoiceToCustomer(AccountingContext context, string organizationId)
    {
        var verification = new JournalEntry(DateTime.Now.Subtract(TimeSpan.FromDays(19)), "Du skickar en faktura");
        verification.OrganizationId = organizationId;

        context.JournalEntries.Add(verification);

        verification.AddDebitEntry(await context.GetAccount(1510), 10000m);
        verification.AddCreditEntry(await context.GetAccount(2610), 2000m);
        verification.AddCreditEntry(await context.GetAccount(3001), 8000m);
    }

    private static async Task TheCustomerPaysTheInvoice(AccountingContext context, string organizationId)
    {
        var verification = new JournalEntry(DateTime.Now.Subtract(TimeSpan.FromDays(19)), "Kunden betalar fakturan");
        verification.OrganizationId = organizationId;

        context.JournalEntries.Add(verification);

        verification.AddDebitEntry(await context.GetAccount(1920), 10000m);
        verification.AddCreditEntry(await context.GetAccount(1510), 10000m);
    }

    private static async Task YouReceiveAInvoice(AccountingContext context, string organizationId)
    {
        var verification = new JournalEntry(DateTime.Now.Subtract(TimeSpan.FromDays(19)), "Du tar emot fakturan");
        verification.OrganizationId = organizationId;

        context.JournalEntries.Add(verification);

        verification.AddDebitEntry(await context.GetAccount(4000), 4000m);
        verification.AddDebitEntry(await context.GetAccount(2640), 1000m);
        verification.AddCreditEntry(await context.GetAccount(2440), 5000m);
    }

    private static async Task YouPayForTheInvoice(AccountingContext context, string organizationId)
    {
        var verification = new JournalEntry(DateTime.Now.Subtract(TimeSpan.FromDays(19)), "Du betalar fakturan");
        verification.OrganizationId = organizationId;

        context.JournalEntries.Add(verification);

        verification.AddDebitEntry(await context.GetAccount(2440), 5000m);
        verification.AddCreditEntry(await context.GetAccount(1930), 5000m);
    }


    private static async Task YouWithdrawMoneyAsSalary(AccountingContext context, string organizationId)
    {
        var verification = new JournalEntry(DateTime.Now.Subtract(TimeSpan.FromDays(19)), "Du tar ut egen lön");
        verification.OrganizationId = organizationId;

        context.JournalEntries.Add(verification);

        verification.AddDebitEntry(await context.GetAccount(2013), 30000m);
        verification.AddCreditEntry(await context.GetAccount(1930), 30000m);
    }

    private static async Task YouTransferFromPlusGiroToCorporateAccount(AccountingContext context, string organizationId)
    {
        var verification = new JournalEntry(DateTime.Now.Subtract(TimeSpan.FromDays(19)), "Du överför pengar från PlusGiro till företagskonto");
        verification.OrganizationId = organizationId;

        context.JournalEntries.Add(verification);

        verification.AddCreditEntry(await context.GetAccount(1920), 10000m);
        verification.AddDebitEntry(await context.GetAccount(1930), 10000m);
    }

    private static async Task YouTransferFromTaxAccountToCorporateAccount(AccountingContext context, string organizationId)
    {
        var verification = new JournalEntry(DateTime.Now.Subtract(TimeSpan.FromDays(19)), "Du överför pengar från skattekonto till företagskonto");
        verification.OrganizationId = organizationId;

        context.JournalEntries.Add(verification);

        verification.AddCreditEntry(await context.GetAccount(1630), 4000m);
        verification.AddDebitEntry(await context.GetAccount(1930), 4000m);
    }
}