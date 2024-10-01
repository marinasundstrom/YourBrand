﻿using YourBrand.Domain;
using YourBrand.Identity;
using YourBrand.Showroom.Domain.Common;
using YourBrand.Tenancy;

namespace YourBrand.Showroom.Domain.Entities;

public class PersonProfileExperience : AuditableEntity, IHasTenant, ISoftDeletable
{
    public string Id { get; set; } = null!;

    public TenantId TenantId { get; set; } = null!;

    //public OrganizationId OrganizationId { get; set; } = null!;

    public PersonProfile PersonProfile { get; set; } = null!;

    public string Title { get; set; } = null!;

    public Company Company { get; set; }

    public string CompanyId { get; set; }

    public string? Location { get; set; }

    public Employment? Employment { get; set; }

    public string? EmploymentType { get; set; } = null!;

    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }

    public bool Current { get; set; }

    public bool Highlight { get; set; }

    public string? Description { get; set; }

    public HashSet<PersonProfileExperienceSkill> Skills { get; private set; } = new HashSet<PersonProfileExperienceSkill>();

    public DateTimeOffset? Deleted { get; set; }
    public UserId? DeletedById { get; set; }
    public User? DeletedBy { get; set; }
}