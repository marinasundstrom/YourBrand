using System.Text.Json;
using System.Text.RegularExpressions;

using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Application.Common.Interfaces;
using YourBrand.Domain.Entities;

namespace YourBrand.Application.Themes;

public record ExportTheme(string Id) : IRequest<string>
{
    public class Handler(IAppServiceContext appServiceContext) : IRequestHandler<ExportTheme, string>
    {
        public async Task<string?> Handle(ExportTheme request, CancellationToken cancellationToken)
        {
            Theme? theme = await appServiceContext.Themes
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

            return JsonSerializer.Serialize(theme);
        }
    }
}
