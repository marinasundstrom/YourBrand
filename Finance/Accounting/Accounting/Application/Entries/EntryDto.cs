using System;

using Accounting.Application.Accounts;
using Accounting.Application.Verifications;

namespace Accounting.Application.Entries;

public record EntryDto(int Id, DateTime Date, VerificationShort Verification, AccountShortDto Account,
    string Description, decimal? Debit, decimal? Credit);