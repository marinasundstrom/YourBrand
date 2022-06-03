
using YourBrand.IdentityService.Domain.Common;
using YourBrand.IdentityService.Domain.Common.Interfaces;

namespace YourBrand.IdentityService.Domain.Entities;

public class TeamMembership : AuditableEntity, ISoftDelete
{
    public string Id { get; set; } = Guid.NewGuid().ToString();

    public Team Team { get; set; } = null!;

    public string TeamId { get; set; } = null!;

    public User User { get; set; } = null!;

    public string UserId { get; set; } = null!;

    public DateTime? Deleted { get; set; }

    public string? DeletedBy { get; set; }
}
