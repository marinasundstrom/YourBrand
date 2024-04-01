using YourBrand.Showroom.Domain.Common;

namespace YourBrand.Showroom.Domain.Entities;

public class CaseProfile : AuditableEntity, ISoftDelete
{
    public string Id { get; set; } = null!;

    public PersonProfile PersonProfile { get; set; } = null!;

    public string? Presentation { get; set; }

    public DateTime? Deleted { get; set; }
    public string? DeletedById { get; set; }
    public User? DeletedBy { get; set; }
}