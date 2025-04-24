using System.Text.Json;

using MediatR;

using YourBrand.Application.Common.Interfaces;
using YourBrand.Domain.Entities;

namespace YourBrand.Application.Themes;

public record ImportTheme(Stream Stream) : IRequest<ThemeDto>
{
    public class Handler(IAppServiceContext appServiceContext) : IRequestHandler<ImportTheme, ThemeDto>
    {
        public async Task<ThemeDto> Handle(ImportTheme request, CancellationToken cancellationToken)
        {
            var theme = JsonSerializer.Deserialize<Theme>(request.Stream);

            appServiceContext.Themes.Add(theme);

            await appServiceContext.SaveChangesAsync(cancellationToken);

            return theme.ToDto();
        }
    }
}