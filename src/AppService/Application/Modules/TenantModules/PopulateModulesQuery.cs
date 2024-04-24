
using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Application.Common.Interfaces;
using YourBrand.Domain.Entities;
using YourBrand.Identity;

namespace YourBrand.Application.Modules.TenantModules;

public record PopulateModulesQuery() : IRequest
{
    public sealed class Handler(IAppServiceContext appServiceContext) : IRequestHandler<PopulateModulesQuery>
    {
        public async Task Handle(PopulateModulesQuery request, CancellationToken cancellationToken)
        {
            var tenantModules = appServiceContext.Modules.Select( module => new TenantModule() {
                Module = module,
                Enabled = false
            });

            appServiceContext.TenantModules.AddRange(tenantModules);

            await appServiceContext.SaveChangesAsync(cancellationToken);
        }
    }
}