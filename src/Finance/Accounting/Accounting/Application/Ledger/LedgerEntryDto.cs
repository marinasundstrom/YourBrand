using YourBrand.Accounting.Application.Accounts;
using YourBrand.Accounting.Application.Journal;

namespace YourBrand.Accounting.Application.Ledger;

public record LedgerEntryDto(int Id, DateTime Date, JournalEntryShort Verification, AccountShortDto Account,
    string Description, decimal? Debit, decimal? Credit);