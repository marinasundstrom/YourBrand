
using TimeReport.Domain.Common;
using TimeReport.Domain.Common.Interfaces;

namespace TimeReport.Domain.Entities;

public class User : AuditableEntity, ISoftDelete
{
    public string Id { get; set; } = null!;

    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string? DisplayName { get; set; }

    public string SSN { get; set; } = null!;

    public string Email { get; set; } = null!;

    public DateTime? Deleted { get; set; }
    public string? DeletedBy { get; set; }
}