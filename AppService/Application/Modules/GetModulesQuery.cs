
using YourBrand.Application.Common.Interfaces;
using YourBrand.Application.Common.Models;
using YourBrand.Domain;
using YourBrand.Identity;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace YourBrand.Application.Modules;

public record GetModulesQuery() : IRequest<IEnumerable<ModuleDto>>
{
    public class Handler : IRequestHandler<GetModulesQuery, IEnumerable<ModuleDto>>
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IAppServiceContext _appServiceContext;

        public Handler(ICurrentUserService currentUserService, IAppServiceContext appServiceContext)
        {
            _currentUserService = currentUserService;
            _appServiceContext = appServiceContext;
        }

        public async Task<IEnumerable<ModuleDto>> Handle(GetModulesQuery request, CancellationToken cancellationToken)
        {
            /*
            return System.Text.Json.JsonSerializer.Deserialize<IEnumerable<ModuleDto>>(
                await File.ReadAllTextAsync("modules.json", cancellationToken)
            )!;
            */
            
            var modules = await _appServiceContext.Modules
            .Where(x => x.Enabled)
            .OrderBy(x => x.Index).ToListAsync(cancellationToken);
                return modules.Select(x => new ModuleDto {
                    Name = x.Name,
                    Assembly = x.Assembly,
                    Enabled = x.Enabled
                });
        }
    }
}