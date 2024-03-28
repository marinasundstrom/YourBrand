using System.ComponentModel.DataAnnotations;

using YourBrand.Accounting.Domain.Enums;

namespace YourBrand.Accounting.Domain.Entities;

public class Verification
{
    [Key]
    public string Id { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string ContentType { get; set; } = null!;

    public AttachmentType Type { get; set; }

    public JournalEntry JournalEntry { get; set; } = null!;

    public DateTime Date { get; set; }

    public string? Description { get; set; }

    public int? InvoiceNo { get; set; }

    public int? ReceiptNo { get; set; }
}