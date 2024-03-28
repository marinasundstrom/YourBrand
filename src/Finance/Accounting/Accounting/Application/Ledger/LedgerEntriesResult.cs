namespace YourBrand.Accounting.Application.Ledger;

public record LedgerEntriesResult(IEnumerable<LedgerEntryDto> Entries, int TotalItems);