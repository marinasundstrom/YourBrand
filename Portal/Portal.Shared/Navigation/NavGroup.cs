namespace YourBrand.Portal.Navigation;

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

    public bool RequiresAuthorization { get; set; }

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
