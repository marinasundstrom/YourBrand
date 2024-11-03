using System.ComponentModel.DataAnnotations;

using YourBrand.Accounting.Application.Accounts;
using YourBrand.Accounting.Application.Journal;
using YourBrand.Accounting.Application.Ledger;
using YourBrand.Accounting.Domain.Entities;

using static YourBrand.Accounting.Application.Shared;

namespace YourBrand.Accounting.Application;

public static class Mappings
{
    public static LedgerEntryDto ToDto(this LedgerEntry e)
    {
        return new LedgerEntryDto(
                    e.Id,
                    e.Date,
                    new JournalEntryShort
                    {
                        Id = e.JournalEntry.Id,
                        Date = e.JournalEntry.Date,
                        Description = e.JournalEntry.Description,
                    },
                    new AccountShortDto
                    {
                        AccountNo = e.Account.AccountNo,
                        Class = new AccountClassDto
                        {
                            Id = (int)e.Account.Class,
                            Description = e.Account.Class.GetAttribute<DisplayAttribute>()!.Name!
                        },
                        Name = e.Account.Name
                    },
                    e.Description,
                    e.Debit,
                    e.Credit
                );
    }

    public static JournalEntryDto ToDto(this JournalEntry v)
    {
        return new JournalEntryDto
        {
            Id = v.Id,
            Date = v.Date,
            Description = v.Description,
            Debit = v.Debit,
            Credit = v.Credit,
            InvoiceNo = v.InvoiceNo,
            Entries = v.Entries.Select(e => e.ToDto()),
            Verifications = v.Verifications.Select(e => e.ToDto())
        };
    }

    public static VerificationDto ToDto(this Verification a)
    {
        return new VerificationDto
        {
            Id = a.Id,
            Name = a.Name,
            ContentType = a.ContentType,
            Description = a.Description,
            InvoiceNo = a.InvoiceNo,
            Url = GetAttachmentUrl(a.Id)!
        };
    }
}
