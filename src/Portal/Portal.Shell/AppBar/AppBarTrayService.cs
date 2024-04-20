using System.Data;

namespace YourBrand.Portal.AppBar;


public interface IAppBarTrayService
{
    IReadOnlyCollection<AppBarTrayItem> Items { get; }

    void AddItem(AppBarTrayItem item);

    void RemoveItem(string id);

    event EventHandler ItemAdded;

    event EventHandler ItemRemoved;

    event EventHandler ItemUpdated;
}

public sealed class AppBarTrayItem
{
    private string? name;
    private Func<string>? nameFunc;
    private string icon;
    private bool isVisible = true;

    public AppBarTrayItem(string id, string name, string icon, Action onClick)
    {
        Id = id;
        Name = name;
        Icon = icon;
        OnClick = onClick;
    }

    public AppBarTrayItem(string id, string name, Type componentType)
    {
        Id = id;
        Name = name;
        ComponentType = componentType;
    }

    public AppBarTrayItem(string id, Func<string> nameFunc, string icon, Action onClick)
    {
        Id = id;
        NameFunc = nameFunc;
        Icon = icon;
        OnClick = onClick;
    }

    public AppBarTrayItem(string id, Func<string> nameFunc, Type componentType)
    {
        Id = id;
        NameFunc = nameFunc;
        ComponentType = componentType;
    }

    public string Id { get; }

    public string Name
    {
        get => name ?? nameFunc?.Invoke() ?? throw new Exception();
        set
        {
            if (value != name)
            {
                name = value;
                Updated?.Invoke(this, EventArgs.Empty);
            }
        }
    }

    public Func<string>? NameFunc
    {
        get => nameFunc;
        set
        {
            if (value != nameFunc)
            {
                name = null;
                nameFunc = value;
                Updated?.Invoke(this, EventArgs.Empty);
            }
        }
    }

    public string Icon
    {
        get => icon;
        set
        {
            if (value != icon)
            {
                icon = value;
                Updated?.Invoke(this, EventArgs.Empty);
            }
        }
    }

    public Type? ComponentType { get; }

    public Action? OnClick { get; }

    public bool IsVisible
    {
        get => isVisible;
        set
        {
            if (value != isVisible)
            {
                isVisible = value;
                Updated?.Invoke(this, EventArgs.Empty);
            }
        }
    }

    public bool RequiresAuthorization { get; set; }

    public IEnumerable<string>? Roles { get; set; }

    public string? Policy { get; set; }

    public event EventHandler Updated = default!;

    public void Hide() => IsVisible = false;

    public void Show() => IsVisible = true;

    public void Refresh() => Updated?.Invoke(this, EventArgs.Empty);
}

public sealed class AppBarTrayService : IAppBarTrayService, IDisposable
{
    private readonly IDictionary<string, AppBarTrayItem> _items = new Dictionary<string, AppBarTrayItem>();

    public IReadOnlyCollection<AppBarTrayItem> Items => _items.Select(x => x.Value).ToList();

    public event EventHandler ItemAdded = default!;

    public event EventHandler ItemRemoved = default!;

    public event EventHandler ItemUpdated = default!;

    void IAppBarTrayService.AddItem(AppBarTrayItem item)
    {
        _items.Add(item.Id, item);
        ItemAdded?.Invoke(this, EventArgs.Empty);

        item.Updated += OnItemUpdated;
    }

    void IAppBarTrayService.RemoveItem(string id)
    {
        var item = _items[id];

        _items.Remove(id);
        ItemRemoved?.Invoke(this, EventArgs.Empty);

        item.Updated -= OnItemUpdated;
    }

    private void OnItemUpdated(object? sender, EventArgs e)
    {
        ItemUpdated?.Invoke(this, EventArgs.Empty);
    }

    public void Dispose()
    {
        foreach (var item in Items)
        {
            item.Updated -= OnItemUpdated;
        }
    }
}