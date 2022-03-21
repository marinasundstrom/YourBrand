using YourCompany.Showroom.Domain.Common;

namespace YourCompany.Showroom.Domain.Entities;

public class Organization : AuditableEntity, ISoftDelete
{
    public string Id { get; set; }

    public string Name { get; set; } = null!;

    public Address Address { get; set; } = null!;

    public ICollection<ConsultantProfile> Consultants { get; set; } = null!;

    public DateTime? Deleted { get; set; }
    public string? DeletedById { get; set; }
    public User? DeletedBy { get; set; }
}