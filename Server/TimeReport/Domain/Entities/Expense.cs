
using TimeReport.Domain.Common;
using TimeReport.Domain.Common.Interfaces;

namespace TimeReport.Domain.Entities;

public class Expense : AuditableEntity, ISoftDelete
{
    public string Id { get; set; } = null!;

    public Project Project { get; set; } = null!;

    public ExpenseType Type { get; set; }

    public DateOnly Date { get; set; }

    public decimal Amount { get; set; }

    public string? Description { get; set; }

    public string? Attachment { get; set; }

    public DateTime? Deleted { get; set; }
    public string? DeletedBy { get; set; }
}