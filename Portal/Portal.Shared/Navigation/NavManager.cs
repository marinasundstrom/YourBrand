namespace YourBrand.Portal.Navigation;

public class NavManager
{
    private List<NavGroup> _groups = new List<NavGroup>();

    public IReadOnlyList<NavGroup> Groups => _groups;

    public NavGroup? GetGroup(string id) => Groups.FirstOrDefault(g => g.Id == id);

    public NavGroup CreateGroup(string id, string name)
    {
        var navGroup = new NavGroup()
        {
            Id = id,
            Name = name
        };
        _groups.Add(navGroup);

        Updated?.Invoke(this, EventArgs.Empty);

        return navGroup;
    }

    public NavGroup CreateGroup(string id, Func<string> name)
    {
        var navGroup = new NavGroup()
        {
            Id = id,
            NameFunc = name
        };
        _groups.Add(navGroup);

        Updated?.Invoke(this, EventArgs.Empty);

        return navGroup;
    }

    public event EventHandler? Updated;
}
