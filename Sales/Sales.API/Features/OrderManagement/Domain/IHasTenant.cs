using YourBrand.Sales.API.Features.OrderManagement.Domain.ValueObjects;

namespace YourBrand.Sales.API.Features.OrderManagement.Domain;

public interface IHasTenant
{
    public TenantId TenantId { get; set; }
}