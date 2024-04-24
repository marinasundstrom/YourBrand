namespace YourBrand.Application.Modules.TenantModules;

public class TenantModuleDto
{
    public Guid Id { get; set; }

    public ModuleDto Module { get; set; }

    public bool Enabled { get; set; }
}