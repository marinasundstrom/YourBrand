using YourBrand.Identity;
using YourBrand.Tenancy;
using YourBrand.Domain;
using YourBrand.Meetings.Domain.ValueObjects;

namespace YourBrand.Meetings.Domain.Entities;

public class Agenda : AggregateRoot<AgendaId>, IAuditable, IHasTenant, IHasOrganization
{
    readonly HashSet<AgendaItem> _agendaItems = new HashSet<AgendaItem>();

    public TenantId TenantId { get; set; }
    public OrganizationId OrganizationId { get; set; }

    public IReadOnlyCollection<AgendaItem> AgendaItems => _agendaItems;
    public MeetingId MeetingId { get; set; }

    public User? CreatedBy { get; set; } = null!;
    public UserId? CreatedById { get; set; } = null!;
    public DateTimeOffset Created { get; set; }
    public User? LastModifiedBy { get; set; }
    public UserId? LastModifiedById { get; set; }
    public DateTimeOffset? LastModified { get; set; }
}
