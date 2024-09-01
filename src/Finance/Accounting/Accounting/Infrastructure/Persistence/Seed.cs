using Microsoft.Extensions.DependencyInjection;

using YourBrand.Accounting.Domain.Entities;
using YourBrand.Tenancy;

namespace YourBrand.Accounting.Infrastructure.Persistence;

public static class Seed
{
    public static bool Run { get; set; } = true;

    public static bool RecreateDatabase { get; set; } = false;

    public static bool SeedAccounts { get; set; } = false;

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
            DoSeedVerifications(context, TenantConstants.OrganizationId);
        }

        await context.SaveChangesAsync();
    }

    private static void DoSeedAccounts(AccountingContext context, string organizationId)
    {
        context.Accounts.AddRange(Accounts.GetAll(organizationId));
    }

    private static void DoSeedVerifications(AccountingContext context, string organizationId)
    {
        InsertMoney(context, organizationId);

        YouSendAnInvoiceToCustomer(context, organizationId);
        TheCustomerPaysTheInvoice(context, organizationId);
        YouReceiveAInvoice(context, organizationId);
        YouTransferFromPlusGiroToCorporateAccount(context, organizationId);
        YouPayForTheInvoice(context, organizationId);
        YouWithdrawMoneyAsSalary(context, organizationId);

        //YouTransferFromTaxAccountToCorporateAccount(context, organizationId);
    }

    private static void InsertMoney(AccountingContext context, string organizationId)
    {
        var verification = new JournalEntry(
            DateTime.Now.Subtract(TimeSpan.FromDays(19)),
            "Du sätter in egna pengar på företagskontot");
        verification.OrganizationId = organizationId;

        context.JournalEntries.Add(verification);

        verification.AddEntries(new[] {
             new LedgerEntry
             {
                 AccountNo = 2018,
                 Description = string.Empty,
                 Credit = 30000m
             },
            new LedgerEntry
            {
                AccountNo = 1930,
                Description = string.Empty,
                Debit = 30000m
            }
        });

        context.SaveChanges();
    }

    private static void YouSendAnInvoiceToCustomer(AccountingContext context, string organizationId)
    {
        var verification = new JournalEntry(DateTime.Now.Subtract(TimeSpan.FromDays(19)), "Du skickar en faktura");
        verification.OrganizationId = organizationId;

        context.JournalEntries.Add(verification);

        verification.AddEntries(new[] {
            new LedgerEntry
            {
                AccountNo = 1510,
                Description = string.Empty,
                Debit = 10000m
            },
            new LedgerEntry
            {
                AccountNo = 2610,
                Description = string.Empty,
                Credit = 2000m
            },
            new LedgerEntry
            {
                AccountNo = 3001,
                Description = string.Empty,
                Credit = 8000m
            }
        });
    }

    private static async void TheCustomerPaysTheInvoice(AccountingContext context, string organizationId)
    {
        var verification = new JournalEntry(DateTime.Now.Subtract(TimeSpan.FromDays(19)), "Kunden betalar fakturan");
        verification.OrganizationId = organizationId;

        context.JournalEntries.Add(verification);

        verification.AddEntries(new[] {
            new LedgerEntry
            {
                AccountNo = 1920,
                Description = string.Empty,
                Debit = 10000m
            },
            new LedgerEntry
            {
                AccountNo = 1510,
                Description = string.Empty,
                Credit = 10000m
            }
        });
    }

    private static void YouReceiveAInvoice(AccountingContext context, string organizationId)
    {
        var verification = new JournalEntry(DateTime.Now.Subtract(TimeSpan.FromDays(19)), "Du tar emot fakturan");
        verification.OrganizationId = organizationId;

        context.JournalEntries.Add(verification);

        verification.AddEntries(new[] {
            new LedgerEntry
            {
                AccountNo = 4000,
                JournalEntryId = verificationId,
                Description = string.Empty,
                Debit = 4000m
            },
            new LedgerEntry
            {
                AccountNo = 2640,
                Description = string.Empty,
                Debit = 1000m
            }, new LedgerEntry
            {
                AccountNo = 2440,
                Description = string.Empty,
                Credit = 5000m
            }
        });
    }

    private static void YouPayForTheInvoice(AccountingContext context, string organizationId)
    {
        var verification = new JournalEntry(DateTime.Now.Subtract(TimeSpan.FromDays(19)), "Du betalar fakturan");
        verification.OrganizationId = organizationId;

        context.JournalEntries.Add(verification);

        verification.AddEntries(new[] {
            new LedgerEntry
            {
                AccountNo = 2440,
                Description = string.Empty,
                Debit = 5000m
            }, new LedgerEntry
            {
                AccountNo = 1930,
                Description = string.Empty,
                Credit = 5000m
            }
        });
    }


    private static void YouWithdrawMoneyAsSalary(AccountingContext context, string organizationId)
    {
        var verification = new JournalEntry(DateTime.Now.Subtract(TimeSpan.FromDays(19)), "Du tar ut egen lön");
        verification.OrganizationId = organizationId;

        context.JournalEntries.Add(verification);

        verification.AddEntries(new[] {
            new LedgerEntry
            {
                AccountNo = 2013,
                Description = string.Empty,
                Debit = 30000m
            },
            new LedgerEntry
            {
                AccountNo = 1930,
                Description = string.Empty,
                Credit = 30000m
            }
        });
    }

    private static void YouTransferFromPlusGiroToCorporateAccount(AccountingContext context, string organizationId)
    {
        var verification = new JournalEntry(DateTime.Now.Subtract(TimeSpan.FromDays(19)), "Du överför pengar från PlusGiro till företagskonto");
        verification.OrganizationId = organizationId;

        context.JournalEntries.Add(verification);

        verification.AddEntries(new[] {
            new LedgerEntry
            {
                AccountNo = 1920,
                Description = string.Empty,
                Credit = 10000m
            },
            new LedgerEntry
            {
                AccountNo = 1930,
                Description = string.Empty,
                Debit = 10000m
            }
        });
    }

    private static void YouTransferFromTaxAccountToCorporateAccount(AccountingContext context, string organizationId)
    {
        var verification = new JournalEntry(DateTime.Now.Subtract(TimeSpan.FromDays(19)), "Du överför pengar från skattekonto till företagskonto");
        verification.OrganizationId = organizationId;

        context.JournalEntries.Add(verification);

        verification.AddEntries(new[] {
            new LedgerEntry
            {
                AccountNo = 1630,
                Description = string.Empty,
                Credit = 4000m
            },
            new LedgerEntry
            {
                AccountNo = 1930,
                Description = string.Empty,
                Debit = 4000m
            }
        });
    }
}