
using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Application.Common.Interfaces;
using YourBrand.Identity;

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

            return await _appServiceContext.Modules
            //.Where(x => x.Enabled)
            .OrderBy(x => x.Index)
            .Select(x => new ModuleDto
            {
                Id = x.Id,
                Name = x.Name,
                Assembly = x.Assembly,
                Enabled = x.Enabled,
                DependantOn = x.DependantOn
            }).ToListAsync(cancellationToken);
        }
    }
}