using System;
using YourBrand.Auditability;
using YourBrand.Domain;
using YourBrand.Identity;
using YourBrand.Meetings.Domain.ValueObjects;
using YourBrand.Tenancy;

namespace YourBrand.Meetings.Domain.Entities;

public class MeetingAttendeeFunction : Entity<MeetingAttendeeFunctionId>, IAuditableEntity<MeetingAttendeeFunctionId>, IHasTenant, IHasOrganization
{
    public MeetingAttendeeFunction()
        : base(new MeetingAttendeeFunctionId())
    {
    }

    public TenantId TenantId { get; set; }

    public OrganizationId OrganizationId { get; set; }

    public MeetingId MeetingId { get; set; }

    public MeetingAttendeeId MeetingAttendeeId { get; set; }

    public MeetingAttendee MeetingAttendee { get; set; } = null!;

    public MeetingFunction Function { get; set; } = null!;

    public int MeetingFunctionId { get; set; }

    public DateTimeOffset AssignedAt { get; set; } = DateTimeOffset.UtcNow;

    public DateTimeOffset? RevokedAt { get; set; }

    public User? CreatedBy { get; set; } = null!;

    public UserId? CreatedById { get; set; } = null!;

    public DateTimeOffset Created { get; set; }

    public User? LastModifiedBy { get; set; }

    public UserId? LastModifiedById { get; set; }

    public DateTimeOffset? LastModified { get; set; }
}
