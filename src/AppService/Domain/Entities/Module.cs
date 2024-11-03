using YourBrand.Domain.Common;

namespace YourBrand.Domain.Entities;

public sealed class Module : Entity<Guid>
{
    public Module()
    {
    }

    public Module(Guid id, string name, string assembly, bool enabled, int index, List<string> dependantOn) : base(id)
    {
        Name = name;
        Assembly = assembly;
        Enabled = enabled;
        Index = index;
        DependantOn = dependantOn;
    }

    public string Name { get; set; }

    public string Assembly { get; set; }

    public bool Enabled { get; set; }

    public int Index { get; set; }

    public List<string> DependantOn { get; set; } = new List<string>();
}