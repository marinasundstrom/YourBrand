using System.Reflection;

namespace YourBrand.Portal.Modules;

public class Module(string name, Assembly assembly)
{
    public string Name { get; } = name;

    public Assembly Assembly { get; } = assembly;

    public bool Enabled { get; set; } = true;
}