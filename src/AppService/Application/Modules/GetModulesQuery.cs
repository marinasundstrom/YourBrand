
using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Application.Common.Interfaces;
using YourBrand.Identity;

namespace YourBrand.Application.Modules;

public record GetModulesQuery() : IRequest<IEnumerable<ModuleDto>>
{
    public class Handler : IRequestHandler<GetModulesQuery, IEnumerable<ModuleDto>>
    {
        private readonly IUserContext _userContext;
        private readonly IAppServiceContext _appServiceContext;

        public Handler(IUserContext userContext, IAppServiceContext appServiceContext)
        {
            _userContext = userContext;
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