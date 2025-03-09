
using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Application.Common.Interfaces;
using YourBrand.Identity;

namespace YourBrand.Application.Themes;

public record GetThemesQuery() : IRequest<IEnumerable<ThemeDto>>
{
    public sealed class Handler(IAppServiceContext appServiceContext) : IRequestHandler<GetThemesQuery, IEnumerable<ThemeDto>>
    {
        public async Task<IEnumerable<ThemeDto>> Handle(GetThemesQuery request, CancellationToken cancellationToken)
        {
            var themes = await appServiceContext.Themes
                .ToListAsync(cancellationToken);

            return themes.Select(x => x.ToDto());
        }
    }
}