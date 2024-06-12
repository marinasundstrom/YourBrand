namespace YourBrand.Portal.Navigation;

public class NavItemOptions
{
    public string Name { get; set; }

    public Func<string> NameFunc { get; set; }

    public void SetName(string name)
    {
        Name = name;
    }

    public void SetName(Func<string> nameFunc)
    {
        NameFunc = nameFunc;
    }

    public string Icon { get; set; }

    public string Href { get; set; }

    public Func<Task> OnClick { get; set; }

    public bool RequiresAuthorization { get; set; }

    public IEnumerable<string>? Roles { get; set; }

    public string? Policy { get; set; }

    public int? Index { get; set; }
}