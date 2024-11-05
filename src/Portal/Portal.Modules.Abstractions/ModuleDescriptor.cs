using System.Reflection;

namespace YourBrand.Portal.Modules;

public sealed class ModuleDescriptor(Guid id, string name, Assembly assembly)
{
    private static readonly Type _moduleInitializerInterface = typeof(IModuleInitializer);

    private Type? _moduleInitializerType;

    public Guid Id { get; } = id;

    public string Name { get; } = name;

    public Assembly Assembly { get; } = assembly;

    public bool Enabled { get; set; } = true;

    public Type? GetInitializerType()
    {
        return _moduleInitializerType ??= this.Assembly
            .GetTypes()
            .FirstOrDefault(type => type.GetInterfaces().Any(i => i == _moduleInitializerInterface));
    }

    public bool Validate() 
    {
        return GetInitializerType() is not null;
    }
}