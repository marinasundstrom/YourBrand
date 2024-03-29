using System.ComponentModel.DataAnnotations.Schema;

using YourBrand.Sales.Features.OrderManagement.Domain.ValueObjects;

namespace YourBrand.Sales.Features.OrderManagement.Domain;

public interface IHasTenant
{
    public TenantId? TenantId { get; set; }
}