
using YourBrand.TimeReport.Domain.Common;
using YourBrand.TimeReport.Domain.Common.Interfaces;
using YourBrand.TimeReport.Domain.Enums;

namespace YourBrand.TimeReport.Domain.Entities;

public class Absence : AuditableEntity, ISoftDelete
{
    public string Id { get; set; } = null!;

    public User User { get; set; } = null!;

    public Project? Project { get; set; }

    public AbsenceType Type { get; set; }

    public AbsenceStatus Status { get; set; }

    public DateOnly? Date { get; set; }

    public decimal Hours { get; set; }

    public bool FullDays { get; set; }

    public DateTime? From { get; set; }

    public DateTime? To { get; set; }

    public string? Note { get; set; }

    public DateTime? Deleted { get; set; }

    public string? DeletedById { get; set; }

    public User? DeletedBy { get; set; }
}