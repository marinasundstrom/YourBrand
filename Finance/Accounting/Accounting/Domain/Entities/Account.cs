using System.ComponentModel.DataAnnotations;

using YourBrand.Accounting.Domain.Enums;

namespace YourBrand.Accounting.Domain.Entities;

public class Account
{
    [Key]
    public int AccountNo { get; set; }

    public AccountClass Class { get; set; }

    public AccountGroup? Group { get; set; }

    public string Name { get; set; } = null!;

    public string Description { get; set; } = null!;

    public List<Entry> Entries { get; set; } = new List<Entry>();
}