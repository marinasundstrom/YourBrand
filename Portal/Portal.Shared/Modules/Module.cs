using System.Reflection;

namespace YourBrand.Portal.Modules;

public class Module
{
    public Module(Assembly assembly)
    {
        Assembly = assembly;
    }

    public Assembly Assembly { get; }

    public bool Enabled { get; set; } = true;
}
