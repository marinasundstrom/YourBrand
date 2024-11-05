using System.Reflection;

namespace YourBrand.Portal.Modules;

public interface IModuleRegistry
{
    IReadOnlyList<ModuleDescriptor> Modules { get; }

    IEnumerable<Assembly> GetEnabledAssemblies();

    IEnumerable<ModuleDescriptor> GetEnabledModules();
}