namespace YourBrand.Accounting.Application.Entries;

public record EntriesResult(IEnumerable<EntryDto> Entries, int TotalItems);