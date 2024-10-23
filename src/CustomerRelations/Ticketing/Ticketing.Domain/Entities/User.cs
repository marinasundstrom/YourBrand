using YourBrand.Identity;
using YourBrand.Tenancy;
using YourBrand.Ticketing.Domain.ValueObjects;

namespace YourBrand.Ticketing.Domain.Entities;

public class User : AggregateRoot<UserId>, IAuditable, IHasTenant
{
    readonly HashSet<OrganizationUser> _organizationUsers = new HashSet<OrganizationUser>();
    readonly HashSet<TeamMembership> _teamMemberships = new HashSet<TeamMembership>();
    readonly HashSet<Organization> _organizations = new HashSet<Organization>();
    readonly HashSet<Team> _teams = new HashSet<Team>();

    public User(UserId id, string name, string email)
        : base(id)
    {
        Id = id;
        Name = name;
        Email = email;
    }

    public string Name { get; set; }

    public TenantId TenantId { get; set; }

    public string Email { get; set; }

    public User? CreatedBy { get; set; }

    public UserId? CreatedById { get; set; }

    public DateTimeOffset Created { get; set; }

    public User? LastModifiedBy { get; set; }

    public UserId? LastModifiedById { get; set; }

    public DateTimeOffset? LastModified { get; set; }

    public IReadOnlyCollection<Organization> Organizations => _organizations;

    public IReadOnlyCollection<Team> Teams => _teams;

    public IReadOnlyCollection<OrganizationUser> OrganizationUsers => _organizationUsers;

    public IReadOnlyCollection<TeamMembership> TeamMemberships => _teamMemberships;
}