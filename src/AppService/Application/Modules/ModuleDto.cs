namespace YourBrand.Application.Modules;

public class ModuleDto
{
    public Guid Id { get; set; }

    public string Name { get; set; }

    public string Assembly { get; set; }

    public bool Enabled { get; set; }

    public IEnumerable<string> DependantOn { get; set; }
}