using YourBrand.Tenancy;

namespace YourBrand.Catalog.Domain.Entities;

public class ProductCategoryAttribute : Entity<Guid>, IHasTenant
{
    public TenantId TenantId { get; set; }

    public long ProductCategoryId { get; set; }

    public ProductCategory ProductCategory { get; set; } = null!;

    //public ProductCategoryAttribute InheritedFromId { get; set; } = null!;

    //public ProductCategoryAttribute InheritedFrom { get; set; } = null!;

    public string AttributeId { get; set; } = null!;

    public Attribute Attribute { get; set; } = null!;
}