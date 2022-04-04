
using YourBrand.IdentityService.Domain.Entities;

namespace YourBrand.IdentityService.Domain.Common.Interfaces;

public interface IAuditableEntity
{
    DateTime Created { get; set; }
    string? CreatedBy { get; set; }
    DateTime? LastModified { get; set; }
    string? LastModifiedBy { get; set; }
}