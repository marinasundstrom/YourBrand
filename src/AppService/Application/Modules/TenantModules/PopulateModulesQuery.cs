
using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Application.Common.Interfaces;
using YourBrand.Domain.Entities;
using YourBrand.Identity;
using YourBrand.Tenancy;

namespace YourBrand.Application.Modules.TenantModules;

public record PopulateModulesQuery() : IRequest
{
    public sealed class Handler(IAppServiceContext appServiceContext, ITenantContext tenantContext) : IRequestHandler<PopulateModulesQuery>
    {
        public async Task Handle(PopulateModulesQuery request, CancellationToken cancellationToken)
        {
            if(appServiceContext.TenantModules.Any(x => x.TenantId == tenantContext.TenantId))
                return;

            var tenantModules = appServiceContext.Modules.Select(module => new TenantModule()
            {
                Module = module,
                Enabled = false
            });

            appServiceContext.TenantModules.AddRange(tenantModules);

            await appServiceContext.SaveChangesAsync(cancellationToken);
        }
    }
}