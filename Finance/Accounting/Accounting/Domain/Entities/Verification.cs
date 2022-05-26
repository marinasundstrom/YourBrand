using System.ComponentModel.DataAnnotations;

namespace Accounting.Domain.Entities;

public class Verification
{
    [Key]
    public int Id { get; set; }

    public DateTime Date { get; set; }

    public string Description { get; set; } = null!;

    public int? InvoiceId { get; set; }

    public int? ReceiptId { get; set; }

    public List<Entry> Entries { get; set; } = new List<Entry>();

    public List<Attachment> Attachments { get; set; } = new List<Attachment>();
}