namespace YourBrand.Accounting.Application.Journal;

public class JournalEntryShort
{
    public int Id { get; set; }

    public DateTime Date { get; set; }

    public string Description { get; set; } = null!;
}