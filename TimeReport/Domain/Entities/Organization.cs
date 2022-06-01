using YourBrand.TimeReport.Domain.Common;
using YourBrand.TimeReport.Domain.Common.Interfaces;

namespace YourBrand.TimeReport.Domain.Entities;

public class Organization : AuditableEntity, ISoftDelete
{
    readonly List<User> _users = new List<User>();
    readonly List<Organization> _subOrganizations = new List<Organization>();
    readonly List<Project> _projects = new List<Project>();
    readonly List<ActivityType> _activityTypes = new List<ActivityType>();
    readonly List<OrganizationUser> _organizationUsers = new List<OrganizationUser>();

    protected Organization()
    {

    }

    public Organization(string name, string? description)
    {
        Id = Guid.NewGuid().ToString();
        Name = name;
        Description = description;
    }

    public ActivityType AddActivityType(string name, string? description)
    {
        ActivityType activityType = new(name, description);
        activityType.Project = null;
        activityType.Organization = this;
        _activityTypes.Add(activityType);
        return activityType;
    }

    public void AddSubOrganization(Organization organization)
    {
        _subOrganizations.Add(organization);
        organization.ParentOrganization = this;
    }

    public void AddProject(Project project)
    {
        _projects.Add(project);
        project.Organization = this;
    }

    public string Id { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public Organization? ParentOrganization { get; private set; }

    public IReadOnlyCollection<User> Users => _users.AsReadOnly();

    public IReadOnlyCollection<Organization> SubOrganizations => _subOrganizations.AsReadOnly();

    public IReadOnlyCollection<Project> Project => _projects.AsReadOnly();

    public IReadOnlyCollection<ActivityType> ActivityTypes => _activityTypes.AsReadOnly();

    public IReadOnlyCollection<OrganizationUser> OrganizationUsers => _organizationUsers.AsReadOnly();

    public DateTime? Deleted { get; set; }
    public string? DeletedById { get; set; }
    public User? DeletedBy { get; set; }
}
