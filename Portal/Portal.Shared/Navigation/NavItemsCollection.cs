namespace YourBrand.Portal.Navigation;

public abstract class NavItemsCollection
{
    private List<INavItem> _items = new List<INavItem>();

    public IReadOnlyList<INavItem> Items => _items;

    public event EventHandler? Updated = default!;

    public IEnumerable<NavGroup> GetGroupsRecursive()
    {
        foreach (var group in Items.OfType<NavGroup>())
        {
            yield return group;

            foreach (var group2 in group.GetGroupsRecursive())
            {
                yield return group2;
            }
        }
    }

    public NavGroup? GetGroup(string id) => _items.OfType<NavGroup>().FirstOrDefault(g => g.Id == id);

    public NavGroup? GetGroup(string id, Action<NavGroupOptions> setup)
    {
        var navGroup = GetGroup(id);

        if (navGroup is null) return null;

        NavGroupOptions options = new NavGroupOptions();
        setup(options);

        navGroup.Name = options.Name;
        navGroup.NameFunc = options.NameFunc;
        navGroup.RequiresAuthorization = options.RequiresAuthorization;
        navGroup.Roles = options.Roles;

        Updated?.Invoke(this, EventArgs.Empty);

        return navGroup;
    }

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

        Updated?.Invoke(this, EventArgs.Empty);

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

        Updated?.Invoke(this, EventArgs.Empty);

        return navItem;
    }

    public NavItem CreateItem(string id, Action<NavItemOptions> setup)
    {
        NavItemOptions options = new NavItemOptions();
        setup(options);

        var navItem = new NavItem()
        {
            Id = id,
            Name = options.Name,
            NameFunc = options.NameFunc,
            Icon = options.Icon,
            Href = options.Href,
            RequiresAuthorization = options.RequiresAuthorization
        };

        if (options.Index is null)
        {
            _items.Add(navItem);
        }
        else
        {
            _items.Insert(options.Index.GetValueOrDefault(), navItem);
        }

        Updated?.Invoke(this, EventArgs.Empty);

        return navItem;
    }

    public NavGroup CreateGroup(string id, string name, string? icon = null)
    {
        var navGroup = new NavGroup()
        {
            Id = id,
            Name = name,
            Icon = icon
        };
        _items.Add(navGroup);

        Updated?.Invoke(this, EventArgs.Empty);

        return navGroup;
    }

    public NavGroup CreateGroup(string id, Func<string> name, string? icon = null)
    {
        var navGroup = new NavGroup()
        {
            Id = id,
            NameFunc = name,
            Icon = icon
        };
        _items.Add(navGroup);

        Updated?.Invoke(this, EventArgs.Empty);

        return navGroup;
    }

    public NavGroup CreateGroup(string id, Action<NavItemOptions> setup)
    {
        NavItemOptions options = new NavItemOptions();
        setup(options);

        var navGroup = new NavGroup()
        {
            Id = id,
            Name = options.Name,
            NameFunc = options.NameFunc,
            Icon = options.Icon,
            RequiresAuthorization = options.RequiresAuthorization
        };

        if (options.Index is null)
        {
            _items.Add(navGroup);
        }
        else
        {
            _items.Insert(options.Index.GetValueOrDefault(), navGroup);
        }

        Updated?.Invoke(this, EventArgs.Empty);

        return navGroup;
    }
}
