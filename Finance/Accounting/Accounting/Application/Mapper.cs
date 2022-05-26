using Accounting.Application.Accounts;
using Accounting.Application.Entries;
using Accounting.Application.Verifications;
using Accounting.Domain.Entities;

using static Accounting.Application.Shared;

namespace Accounting.Application;

public static class Mappings
{
    public static EntryDto ToDto(this Entry e)
    {
        return new EntryDto(
                    e.Id,
                    e.Date,
                    new VerificationShort
                    {
                        Id = e.Verification.Id,
                        Date = e.Verification.Date,
                        Description = e.Verification.Description,
                    },
                    new AccountShortDto
                    {
                        AccountNo = e.Account.AccountNo,
                        Name = e.Account.Name
                    },
                    e.Description,
                    e.Debit,
                    e.Credit
                );
    }

    public static VerificationDto ToDto(this Verification v)
    {
        return new VerificationDto
        {
            Id = v.Id,
            Date = v.Date,
            Description = v.Description,
            Debit = v.Entries.Sum(e => e.Debit.GetValueOrDefault()),
            Credit = v.Entries.Sum(e => e.Credit.GetValueOrDefault()),
            InvoiceId = v.InvoiceId,
            Attachments = v.Attachments.Select(e => e.ToDto())
        };
    }

    public static AttachmentDto ToDto(this Attachment a)
    {
        return new AttachmentDto
        {
            Id = a.Id,
            Name = a.Name,
            ContentType = a.ContentType,
            Description = a.Description,
            InvoiceId = a.InvoiceId,
            Url = GetAttachmentUrl(a.Id)!
        };
    }
}