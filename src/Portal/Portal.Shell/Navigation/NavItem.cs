namespace YourBrand.Portal.Navigation;

public class NavItem : INavItem
{
    private string? name;

    public string Id { get; set; } = null!;

    public string Name
    {
        get => name ?? NameFunc?.Invoke() ?? throw new Exception();
        set => name = value;
    }

    public Func<string>? NameFunc { get; set; }

    public void SetName(string name)
    {
        Name = name;
    }

    public void SetName(Func<string> nameFunc)
    {
        NameFunc = nameFunc;
    }

    public string? Icon { get; set; }

    public string Href { get; set; } = null!;

    public Func<Task> OnClick { get; set; }

    public bool Visible { get; set; } = true;

    public bool RequiresAuthorization { get; set; }

    public IEnumerable<string>? Roles { get; set; }

    public string? Policy { get; set; }

    public Action? Updated { get; set; }
}