using YourBrand.Accounting.Application.Accounts;
using YourBrand.Accounting.Application.Entries;
using YourBrand.Accounting.Application.Verifications;
using YourBrand.Accounting.Domain.Entities;

using static YourBrand.Accounting.Application.Shared;

namespace YourBrand.Accounting.Application;

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