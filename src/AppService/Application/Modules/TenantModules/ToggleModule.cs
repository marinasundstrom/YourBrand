
using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Application.Common.Interfaces;
using YourBrand.Domain.Entities;
using YourBrand.Identity;

namespace YourBrand.Application.Modules.TenantModules;

public record ToggleModule(Guid Id) : IRequest
{
    public class Handler(IAppServiceContext appServiceContext) : IRequestHandler<ToggleModule>
    {
        public async Task Handle(ToggleModule request, CancellationToken cancellationToken)
        {
            var tenantModule = await appServiceContext.TenantModules
                .Include(x => x.Module)
                .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

            if (tenantModule is not null)
            {
                tenantModule.Enabled = !tenantModule.Enabled;

                if (tenantModule.Enabled)
                {
                    await EnableModuleDependencies(appServiceContext, tenantModule, cancellationToken);
                }

                await appServiceContext.SaveChangesAsync(cancellationToken);
            }
        }

        private static async Task EnableModuleDependencies(IAppServiceContext appServiceContext, TenantModule? tenantModule, CancellationToken cancellationToken)
        {
            var module = tenantModule.Module;

            if(module.DependantOn is null)
                return;

            foreach (var dependencyModuleName in module.DependantOn)
            {
                await EnableModuleDependency(appServiceContext, dependencyModuleName, cancellationToken);
            }
        }

        private static async Task EnableModuleDependency(IAppServiceContext appServiceContext, string dependencyModuleName, CancellationToken cancellationToken)
        {
            var dependencyModule = await appServiceContext.TenantModules
                .Include(x => x.Module)
                .FirstOrDefaultAsync(x => x.Module.Assembly == dependencyModuleName, cancellationToken);

            if (dependencyModule is not null)
            {
                if(dependencyModule.Enabled)
                    return;

                dependencyModule.Enabled = true;

                var module = dependencyModule.Module;

                if (module.DependantOn is null)
                    return;

                foreach (var dependencyModuleName2 in module.DependantOn)
                {
                    await EnableModuleDependency(appServiceContext, dependencyModuleName2, cancellationToken);
                }
            }
        }
    }
}