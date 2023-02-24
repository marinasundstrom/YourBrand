namespace YourBrand.Portal.Navigation;

public class NavGroup : NavItemsCollection, INavItem
{
    private string? name;

    public string Id { get; set; } = null!;

    public string? Icon { get; set; }

    public string Name
    {
        get => name ?? NameFunc?.Invoke() ?? throw new Exception();
        set => name = value;
    }

    public Func<string>? NameFunc { get; set; }

    public bool Expanded { get; set; }

    public bool Visible { get; set; } = true;

    public bool RequiresAuthorization { get; set; }

    public IEnumerable<string>? Roles { get; set; }

}
