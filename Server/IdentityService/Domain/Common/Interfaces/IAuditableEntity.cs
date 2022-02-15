
using Skynet.IdentityService.Domain.Entities;

namespace Skynet.IdentityService.Domain.Common.Interfaces;

public interface IAuditableEntity
{
    DateTime Created { get; set; }
    string CreatedBy { get; set; }
    DateTime? LastModified { get; set; }
    string? LastModifiedBy { get; set; }
}