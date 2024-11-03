﻿using YourBrand.Domain;
using YourBrand.Identity;
using YourBrand.Showroom.Domain.Common;
using YourBrand.Showroom.Domain.Enums;
using YourBrand.Tenancy;

namespace YourBrand.Showroom.Domain.Entities;

public class Case : AuditableEntity<string>, IHasTenant, ISoftDeletable
{
    public Case()
        : base(Guid.NewGuid().ToString())
    {

    }

    public TenantId TenantId { get; set; } = null!;

    public OrganizationId OrganizationId { get; set; } = null!;

    public string? Description { get; set; } = null!;

    public CaseStatus Status { get; set; }

    public ICollection<CaseProfile> CaseProfiles { get; set; } = null!;

    public CasePricing Pricing { get; set; }

    public bool IsDeleted { get; set; }
    public DateTimeOffset? Deleted { get; set; }
    public UserId? DeletedById { get; set; }
    public User? DeletedBy { get; set; }
}

public class Location : AuditableEntity<string>, ISoftDeletable
{
    public string? CityOrDistrict { get; set; }

    public string? Country { get; set; }

    public bool IsDeleted { get; set; }
    public DateTimeOffset? Deleted { get; set; }
    public UserId? DeletedById { get; set; }
    public User? DeletedBy { get; set; }
}

public class CasePricing
{
    public decimal? HourlyPrice { get; set; }
    public double? Hours { get; set; }
    public decimal? Total { get; set; }
}