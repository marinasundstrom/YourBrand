
using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Application.Common.Interfaces;
using YourBrand.Identity;

namespace YourBrand.Application.Modules.TenantModules;

public record GetModulesQuery() : IRequest<IEnumerable<TenantModuleDto>>
{
    public sealed class Handler(IAppServiceContext appServiceContext) : IRequestHandler<GetModulesQuery, IEnumerable<TenantModuleDto>>
    {
        public async Task<IEnumerable<TenantModuleDto>> Handle(GetModulesQuery request, CancellationToken cancellationToken)
        {
            /*
            return System.Text.Json.JsonSerializer.Deserialize<IEnumerable<ModuleDto>>(
                await File.ReadAllTextAsync("modules.json", cancellationToken)
            )!;
            */

            return await appServiceContext.TenantModules
                .Include(x => x.Module)
                //.Where(x => x.Enabled)
                .OrderBy(x => x.Module.Index)
                .Select(x => new TenantModuleDto
                {
                    Id = x.Id,
                    Module = new ModuleDto
                    {
                        Id = x.Module.Id,
                        Name = x.Module.Name,
                        Assembly = x.Module.Assembly,
                        Enabled = x.Module.Enabled,
                        DependantOn = x.Module.DependantOn
                    },
                    Enabled = x.Enabled,
                }).ToListAsync(cancellationToken);
        }
    }
}