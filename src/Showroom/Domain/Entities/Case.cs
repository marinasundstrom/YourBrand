using YourBrand.Showroom.Domain.Common;
using YourBrand.Showroom.Domain.Enums;
using YourBrand.Tenancy;
using YourBrand.Domain;

namespace YourBrand.Showroom.Domain.Entities;

public class Case : AuditableEntity, IHasTenant, ISoftDelete
{
    public string Id { get; set; } = null!;

    public TenantId TenantId { get; set; } = null!;


    public string? Description { get; set; } = null!;

    public CaseStatus Status { get; set; }

    public ICollection<CaseProfile> CaseProfiles { get; set; } = null!;

    public CasePricing Pricing { get; set; }

    public DateTimeOffset? Deleted { get; set; }
    public string? DeletedById { get; set; }
    public User? DeletedBy { get; set; }
}

public class Location : AuditableEntity, ISoftDelete
{
    public string Id { get; set; } = null!;

    public string? CityOrDistrict { get; set; }

    public string? Country { get; set; }

    public DateTimeOffset? Deleted { get; set; }
    public string? DeletedById { get; set; }
    public User? DeletedBy { get; set; }
}

public class CasePricing
{
    public decimal? HourlyPrice { get; set; }
    public double? Hours { get; set; }
    public decimal? Total { get; set; }
}