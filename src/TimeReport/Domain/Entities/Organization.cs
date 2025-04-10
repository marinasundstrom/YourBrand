using YourBrand.Domain;
using YourBrand.Identity;
using YourBrand.Tenancy;
using YourBrand.TimeReport.Domain.Common;

namespace YourBrand.TimeReport.Domain.Entities;

public class Organization : AuditableEntity<OrganizationId>, IOrganization, ISoftDeletableWithAudit<User>, IHasTenant
{
    readonly HashSet<User> _users = new HashSet<User>();
    readonly HashSet<Organization> _subOrganizations = new HashSet<Organization>();
    readonly HashSet<Project> _projects = new HashSet<Project>();
    readonly HashSet<TaskType> _taskTypes = new HashSet<TaskType>();
    readonly HashSet<OrganizationUser> _organizationUsers = new HashSet<OrganizationUser>();

    protected Organization()
    {

    }

    public Organization(OrganizationId id, string name, string? description) : base(id)
    {
        Name = name;
        Description = description;
    }

    public TaskType AddTaskType(string name, string? description)
    {
        TaskType taskType = new(name, description);
        taskType.Project = null;
        taskType.Organization = this;
        _taskTypes.Add(taskType);
        return taskType;
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

    public TenantId TenantId { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public void AddUser(User user)
    {
        _users.Add(user);
    }

    public Organization? ParentOrganization { get; private set; }

    public double NormalWorkingHours { get; set; }

    public IReadOnlyCollection<User> Users => _users;

    public IReadOnlyCollection<Organization> SubOrganizations => _subOrganizations;

    public IReadOnlyCollection<Project> Project => _projects;

    public IReadOnlyCollection<TaskType> TaskTypes => _taskTypes;

    public IReadOnlyCollection<OrganizationUser> OrganizationUsers => _organizationUsers;

    public bool IsDeleted { get; set; }

    public DateTimeOffset? Deleted { get; set; }

    public UserId? DeletedById { get; set; }

    public User? DeletedBy { get; set; }

    public bool HasUser(UserId userId)
    {
        return Users.Any(x => x.Id == userId);
    }
}