﻿
using YourBrand.Domain;
using YourBrand.Identity;
using YourBrand.Tenancy;
using YourBrand.TimeReport.Domain.Common;
using YourBrand.TimeReport.Domain.Common.Interfaces;

namespace YourBrand.TimeReport.Domain.Entities;

public class Expense : AuditableEntity, IHasTenant, IHasOrganization, ISoftDeletable
{
    public string Id { get; set; } = null!;

    public TenantId TenantId { get; set; }

    public OrganizationId OrganizationId { get; set; } = null!;

    public Project Project { get; set; } = null!;

    public ExpenseType ExpenseType { get; set; } = null!;

    public DateOnly Date { get; set; }

    public decimal Amount { get; set; }

    public string? Description { get; set; }

    public string? Attachment { get; set; }

    public DateTime? Deleted { get; set; }

    public UserId? DeletedById { get; set; }

    public User? DeletedBy { get; set; }
}