
using YourCompany.TimeReport.Domain.Common;
using YourCompany.TimeReport.Domain.Common.Interfaces;

namespace YourCompany.TimeReport.Domain.Entities;

public class TimeSheetActivity : AuditableEntity, ISoftDelete
{
    public string Id { get; set; } = null!;

    public TimeSheet TimeSheet { get; set; } = null!;

    public Project Project { get; set; } = null!;

    public Activity Activity { get; set; } = null!;

    public List<Entry> Entries { get; set; } = new List<Entry>();

    // public decimal? HourlyRate { get; set; }

    public DateTime? Deleted { get; set; }
    public string? DeletedById { get; set; }
    public User? DeletedBy { get; set; }
}