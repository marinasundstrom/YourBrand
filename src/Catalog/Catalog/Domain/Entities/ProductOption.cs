﻿using YourBrand.Domain;
using YourBrand.Tenancy;

namespace YourBrand.Catalog.Domain.Entities;

public class ProductOption : Entity<string>, IHasTenant, IHasOrganization
{
    public ProductOption(string id) : base(id)
    {
    }

    public ProductOption() : base(Guid.NewGuid().ToString())
    {
    }

    public TenantId TenantId { get; set; }

    public OrganizationId OrganizationId { get; set; }

    public int ProductId { get; set; }

    public Product Product { get; set; } = null!;

    public string OptionId { get; set; } = null!;

    public Option Option { get; set; } = null!;

    //public bool IsSelected { get; set; }

    //public bool IsRequired { get; set; }

    public bool? IsInherited { get; set; }

    // Add fields for default values
}