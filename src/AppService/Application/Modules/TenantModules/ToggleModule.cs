
using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Application.Common.Interfaces;
using YourBrand.Identity;

namespace YourBrand.Application.Modules.TenantModules;

public record ToggleModule(Guid Id) : IRequest
{
    public class Handler(IAppServiceContext appServiceContext) : IRequestHandler<ToggleModule>
    {
        public async Task Handle(ToggleModule request, CancellationToken cancellationToken)
        {
            var module = await appServiceContext.TenantModules.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

            if (module is not null)
            {
                module.Enabled = !module.Enabled;

                await appServiceContext.SaveChangesAsync(cancellationToken);
            }
        }
    }
}