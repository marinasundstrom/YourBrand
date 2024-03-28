using System.ComponentModel.DataAnnotations;

namespace YourBrand.Accounting.Application.Journal;

public class CreateEntry
{
    [Required]
    public int AccountNo { get; set; }

    public string? Description { get; set; }

    public decimal? Debit { get; set; }

    public decimal? Credit { get; set; }
}