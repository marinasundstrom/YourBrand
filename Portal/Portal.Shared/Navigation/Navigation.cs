namespace YourBrand.Portal.Navigation;

public class NavManager 
{
    public List<NavGroup> Groups { get; set; } = new List<NavGroup>();

    public NavGroup? GetGroup(string id) => Groups.FirstOrDefault(g => g.Id == id);

    public NavGroup AddGroup(string id, string name)
    {
        var navGroup = new NavGroup() {
            Id = id,
            Name = name
        };
        Groups.Add(navGroup);
        return navGroup;
    }
}

public class NavGroup 
{
    public string Id { get; set; } = null!;

    public string Name { get; set; } = null!;

    public List<NavItem> Items { get; set; } = new List<NavItem>();

    public bool Expanded { get; set; }

    public bool RequireAuthorization { get; set; }

    public string? Roles { get; set; }

    public NavItem AddItem(string id, string name, string icon, string href) 
    {
        var navItem = new NavItem() {
            Id = id,
            Name = name,
            Icon = icon,
            Href = href
        };
        Items.Add(navItem);
        return navItem;
    }
}

public class NavItem
{
    public string Id { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string? Icon { get; set; }

    public string Href { get; set; } = null!;

    public bool RequireAuthorization { get; set; }

    public string? Roles { get; set; }
}