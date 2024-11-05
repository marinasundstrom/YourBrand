using System.Reflection;

namespace YourBrand.Portal.Modules;

public sealed class ModuleRegistry : IModuleRegistry
{
    private readonly List<ModuleDescriptor> _modules = new List<ModuleDescriptor>();

    public IReadOnlyList<ModuleDescriptor> Modules => _modules;

    public ModuleDescriptor RegisterModule(Guid id, string name, Assembly assembly, bool enabled)
    {
        var module = new ModuleDescriptor(id, name, assembly) { Enabled = enabled };

        _modules.Add(module);

        return module;
    }

    public IEnumerable<Assembly> GetEnabledAssemblies()
    {
        return _modules.Where(m => m.Enabled).Select(m => m.Assembly);
    }

    public IEnumerable<ModuleDescriptor> GetEnabledModules()
    {
        return _modules.Where(m => m.Enabled);
    }
}
