namespace YourBrand.Portal.Navigation;

public class NavManager
{
    private List<NavGroup> _groups = new List<NavGroup>();

    public IReadOnlyList<NavGroup> Groups => _groups;

    public NavGroup? GetGroup(string id) => Groups.FirstOrDefault(g => g.Id == id);

    public NavGroup AddGroup(string id, string name)
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

    public event EventHandler? Updated;
}

public class NavGroup
{
    private List<NavItem> _items = new List<NavItem>();

    public string Id { get; set; } = null!;

    public string Name { get; set; } = null!;

    public IReadOnlyList<NavItem> Items => _items;

    public bool Expanded { get; set; }

    public bool Visible { get; set; } = true;

    public bool RequireAuthorization { get; set; }

    public string? Roles { get; set; }

    public NavItem AddItem(string id, string name, string icon, string href)
    {
        var navItem = new NavItem()
        {
            Id = id,
            Name = name,
            Icon = icon,
            Href = href
        };
        _items.Add(navItem);
        return navItem;
    }
}

public class NavItem
{
    public string Id { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string? Icon { get; set; }

    public string Href { get; set; } = null!;

    public bool Visible { get; set; } = true;

    public bool RequireAuthorization { get; set; }

    public string? Roles { get; set; }
}