
using YourCompany.IdentityService.Domain.Entities;

namespace YourCompany.IdentityService.Domain.Common.Interfaces;

public interface IAuditableEntity
{
    DateTime Created { get; set; }
    string? CreatedBy { get; set; }
    DateTime? LastModified { get; set; }
    string? LastModifiedBy { get; set; }
}