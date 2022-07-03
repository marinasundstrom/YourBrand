using System.Reflection;

namespace YourBrand.Portal;

public class Module
{
    public Module(Assembly assembly)
    {
        Assembly = assembly;
    }

    public Assembly Assembly { get; }

    public bool Enabled { get; init; } = true;
}
