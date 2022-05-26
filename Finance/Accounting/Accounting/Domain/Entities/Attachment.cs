using System.ComponentModel.DataAnnotations;

using Accounting.Domain.Enums;

namespace Accounting.Domain.Entities;

public class Attachment
{
    [Key]
    public string Id { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string ContentType { get; set; } = null!;

    public AttachmentType Type { get; set; }

    public Verification Verification { get; set; } = null!;

    public DateTime Date { get; set; }

    public string? Description { get; set; }

    public int? InvoiceId { get; set; }

    public int? ReceiptId { get; set; }
}