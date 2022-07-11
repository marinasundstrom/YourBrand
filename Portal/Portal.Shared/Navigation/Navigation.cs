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

public class NavGroup
{
    private string? name;

    private List<NavItem> _items = new List<NavItem>();

    public string Id { get; set; } = null!;

    public string Name 
    { 
        get => name ?? NameFunc?.Invoke() ?? throw new Exception();
        set => name = value;
    }

    public Func<string>? NameFunc { get; set; }

    public IReadOnlyList<NavItem> Items => _items;

    public bool Expanded { get; set; }

    public bool Visible { get; set; } = true;

    public bool RequireAuthorization { get; set; }

    public string? Roles { get; set; }

    public NavItem CreateItem(string id, string name, string icon, string href)
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

    public NavItem CreateItem(string id, Func<string> name, string icon, string href)
    {
        var navItem = new NavItem()
        {
            Id = id,
            NameFunc = name,
            Icon = icon,
            Href = href
        };
        _items.Add(navItem);
        return navItem;
    }
}

public class NavItem
{
    private string? name;

    public string Id { get; set; } = null!;

    public string Name 
    { 
        get => name ?? NameFunc?.Invoke() ?? throw new Exception();
        set => name = value;
    }

    public Func<string>? NameFunc { get; set; }

    public string? Icon { get; set; }

    public string Href { get; set; } = null!;

    public bool Visible { get; set; } = true;

    public bool RequireAuthorization { get; set; }

    public string? Roles { get; set; }
}