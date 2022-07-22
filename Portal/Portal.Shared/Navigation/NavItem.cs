namespace YourBrand.Portal.Navigation;

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

    public bool Visible { get; set; } = true;

    public bool RequiresAuthorization { get; set; }

    public IEnumerable<string>? Roles { get; set; }
}