using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using YourBrand.Accounting.Domain.Common;

namespace YourBrand.Accounting.Domain.Entities;

public class Entry : Entity
{
    public Entry()
    {

    }

    public Entry(DateTime date, Account account, decimal? debit, decimal? credit, string? description)
    {
        Date = date;
        Account = account;
        Debit = debit;
        Credit = credit;
        Description = description;
    }

    [Key]
    public int Id { get; set; }

    public DateTime Date { get; set; } = DateTime.Now;

    public int VerificationId { get; set; }

    [ForeignKey(nameof(VerificationId))]
    public Verification Verification { get; set; } = null!;

    public int AccountNo { get; set; }

    [ForeignKey(nameof(AccountNo))]
    public Account Account { get; set; } = null!;

    public string? Description { get; set; } = null!;

    public decimal? Debit { get; set; }

    public decimal? Credit { get; set; }
}