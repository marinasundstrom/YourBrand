
using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Application.Common.Interfaces;
using YourBrand.Application.Common.Models;
using YourBrand.Domain;
using YourBrand.Identity;

namespace YourBrand.Application.Modules;

public record ToggleModule(Guid Id) : IRequest
{
    public class Handler : IRequestHandler<ToggleModule>
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IAppServiceContext _appServiceContext;

        public Handler(ICurrentUserService currentUserService, IAppServiceContext appServiceContext)
        {
            _currentUserService = currentUserService;
            _appServiceContext = appServiceContext;
        }

        public async Task Handle(ToggleModule request, CancellationToken cancellationToken)
        {
            var module = await _appServiceContext.Modules.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

            if(module is not null) 
            {
                module.Enabled = !module.Enabled;

                await _appServiceContext.SaveChangesAsync(cancellationToken);
            }
        }
    }
}