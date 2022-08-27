using YourBrand.Accounting.Domain.Entities;
using YourBrand.Accounting.Domain.Enums;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

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
        using var context = scope.ServiceProvider.GetRequiredService<AccountingContext>();

        if (RecreateDatabase)
        {
            await context.Database.EnsureDeletedAsync();
            await context.Database.EnsureCreatedAsync();
        }

        if (SeedAccounts)
        {
            DoSeedAccounts(context);
        }

        if (SeedVerifications)
        {
            DoSeedVerifications(context);
        }

        await context.SaveChangesAsync();
    }

    private static void DoSeedAccounts(AccountingContext context)
    {
        context.Accounts.AddRange(Accounts.GetAll());
    }

    private static void DoSeedVerifications(AccountingContext context)
    {
        InsertMoney(context);
        
        YouSendAnInvoiceToCustomer(context);
        TheCustomerPaysTheInvoice(context);
        YouReceiveAInvoice(context);
        YouTransferFromPlusGiroToCorporateAccount(context);
        YouPayForTheInvoice(context);
        YouWithdrawMoneyAsSalary(context);

        //YouTransferFromTaxAccountToCorporateAccount(context);
    }

    private static void InsertMoney(AccountingContext context)
    {
        var verification = new Verification(
            DateTime.Now.Subtract(TimeSpan.FromDays(19)), 
            "Du sätter in egna pengar på företagskontot");

        context.Verifications.Add(verification);
        
        verification.AddEntries(new[] {
             new Entry
             {
                 AccountNo = 2018,
                 Description = string.Empty,
                 Credit = 30000m
             },
            new Entry
            {
                AccountNo = 1930,
                Description = string.Empty,
                Debit = 30000m
            }
        });

        context.SaveChanges();
    }

    private static void YouSendAnInvoiceToCustomer(AccountingContext context)
    {
        var verification = new Verification(DateTime.Now.Subtract(TimeSpan.FromDays(19)), "Du skickar en faktura");

        context.Verifications.Add(verification);

        verification.AddEntries(new[] {
            new Entry
            {
                AccountNo = 1510,
                Description = string.Empty,
                Debit = 10000m
            },
            new Entry
            {
                AccountNo = 2610,
                Description = string.Empty,
                Credit = 2000m
            },
            new Entry
            {
                AccountNo = 3001,
                Description = string.Empty,
                Credit = 8000m
            }
        });
    }

    private static async void TheCustomerPaysTheInvoice(AccountingContext context)
    {
        var verification = new Verification(DateTime.Now.Subtract(TimeSpan.FromDays(19)), "Kunden betalar fakturan");

        context.Verifications.Add(verification);

        verification.AddEntries(new[] {
            new Entry
            {
                AccountNo = 1920,
                Description = string.Empty,
                Debit = 10000m
            },
            new Entry
            {
                AccountNo = 1510,
                Description = string.Empty,
                Credit = 10000m
            }
        });
    }

    private static void YouReceiveAInvoice(AccountingContext context)
    {
        var verification = new Verification(DateTime.Now.Subtract(TimeSpan.FromDays(19)), "Du tar emot fakturan");

        context.Verifications.Add(verification);

        verification.AddEntries(new[] {
            new Entry
            {
                AccountNo = 4000,
                VerificationId = verificationId,
                Description = string.Empty,
                Debit = 4000m
            },
            new Entry
            {
                AccountNo = 2640,
                Description = string.Empty,
                Debit = 1000m
            }, new Entry
            {
                AccountNo = 2440,
                Description = string.Empty,
                Credit = 5000m
            }
        });
    }

    private static void YouPayForTheInvoice(AccountingContext context)
    {
        var verification = new Verification(DateTime.Now.Subtract(TimeSpan.FromDays(19)), "Du betalar fakturan");

        context.Verifications.Add(verification);

        verification.AddEntries(new[] {
            new Entry
            {
                AccountNo = 2440,
                Description = string.Empty,
                Debit = 5000m
            }, new Entry
            {
                AccountNo = 1930,
                Description = string.Empty,
                Credit = 5000m
            }
        });
    }


    private static void YouWithdrawMoneyAsSalary(AccountingContext context)
    {
        var verification = new Verification(DateTime.Now.Subtract(TimeSpan.FromDays(19)), "Du tar ut egen lön");

        context.Verifications.Add(verification);

        verification.AddEntries(new[] {
            new Entry
            {
                AccountNo = 2013,
                Description = string.Empty,
                Debit = 30000m
            },
            new Entry
            {
                AccountNo = 1930,
                Description = string.Empty,
                Credit = 30000m
            }
        });
    }

    private static void YouTransferFromPlusGiroToCorporateAccount(AccountingContext context)
    {
        var verification = new Verification(DateTime.Now.Subtract(TimeSpan.FromDays(19)), "Du överför pengar från PlusGiro till företagskonto");

        context.Verifications.Add(verification);

        verification.AddEntries(new[] {
            new Entry
            {
                AccountNo = 1920,
                Description = string.Empty,
                Credit = 10000m
            },
            new Entry
            {
                AccountNo = 1930,
                Description = string.Empty,
                Debit = 10000m
            }
        });
    }

    private static void YouTransferFromTaxAccountToCorporateAccount(AccountingContext context)
    {
        var verification = new Verification(DateTime.Now.Subtract(TimeSpan.FromDays(19)), "Du överför pengar från skattekonto till företagskonto");

        context.Verifications.Add(verification);

        verification.AddEntries(new[] {
            new Entry
            {
                AccountNo = 1630,
                Description = string.Empty,
                Credit = 4000m
            },
            new Entry
            {
                AccountNo = 1930,
                Description = string.Empty,
                Debit = 4000m
            }
        });
    }
}