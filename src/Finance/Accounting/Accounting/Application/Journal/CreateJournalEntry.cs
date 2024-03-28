using System.ComponentModel.DataAnnotations;

namespace YourBrand.Accounting.Application.Journal;

public class CreateJournalEntry
{
    public string Description { get; set; } = null!;

    public int? InvoiceNo { get; set; }

    [Required]
    public List<CreateEntry> Entries { get; set; } = new List<CreateEntry>()!;
}