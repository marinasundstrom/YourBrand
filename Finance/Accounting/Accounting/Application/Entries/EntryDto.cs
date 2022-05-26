using System;

using YourBrand.Accounting.Application.Accounts;
using YourBrand.Accounting.Application.Verifications;

namespace YourBrand.Accounting.Application.Entries;

public record EntryDto(int Id, DateTime Date, VerificationShort Verification, AccountShortDto Account,
    string Description, decimal? Debit, decimal? Credit);