using YourBrand.Domain.Common;

namespace YourBrand.Domain.Entities;

public sealed class Module : Entity
{
    public Guid Id { get; set; } = default!;

    public string Name { get; set; }

    public string Assembly { get; set; }

    public bool Enabled { get; set; }

    public int Index { get; set; }

    public List<string> DependantOn { get; set; } = new List<string>();
}