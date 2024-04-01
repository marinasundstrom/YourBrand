namespace YourBrand.Accounting.Application.Journal;

public record JournalEntryResult(IEnumerable<JournalEntryDto> Verifications, int TotalItems);