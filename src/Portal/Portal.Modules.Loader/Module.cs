using System.Reflection;

namespace YourBrand.Portal.Modules;

public class Module
{
    public Module(string name, Assembly assembly)
    {
        Name = name;
        Assembly = assembly;
    }

    public string Name { get; }

    public Assembly Assembly { get; }

    public bool Enabled { get; set; } = true;
}
