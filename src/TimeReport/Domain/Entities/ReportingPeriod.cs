﻿
using YourBrand.Domain;
using YourBrand.Identity;
using YourBrand.Tenancy;
using YourBrand.TimeReport.Domain.Common;

namespace YourBrand.TimeReport.Domain.Entities;

public class ReportingPeriod : AuditableEntity, IHasTenant, IHasOrganization
{
    private readonly HashSet<Entry> _entries = new HashSet<Entry>();

    public ReportingPeriod(User user, int year, int month)
    {
        Id = Guid.NewGuid().ToString();
        User = user;
        Year = year;
        Month = month;
        Status = EntryStatus.Unlocked;
    }

    internal ReportingPeriod()
    {
    }

    public string Id { get; set; } = null!;

    public TenantId TenantId { get; set; }

    public OrganizationId OrganizationId { get; set; } = null!;

    public User User { get; private set; } = null!;

    public UserId UserId { get; private set; } = null!;

    public int Year { get; private set; }

    public int Month { get; private set; }

    public IReadOnlyCollection<Entry> Entries => _entries;

    public EntryStatus Status { get; private set; }

    public void UpdateStatus(EntryStatus status)
    {
        Status = status;
    }

    public void AddEntry(Entry entry)
    {
        _entries.Add(entry);
    }
}