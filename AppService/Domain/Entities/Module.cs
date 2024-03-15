
using YourBrand.Domain.Common;
using System.Text.Json;

namespace YourBrand.Domain.Entities;

public sealed class Module : Entity
{
    public Guid Id { get; set; } = default!;

    public string Name { get; set; }

    public string Assembly { get; set; }

    public bool Enabled { get; set; }
}