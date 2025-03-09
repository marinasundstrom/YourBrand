using System.Text.RegularExpressions;

using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Application.Common.Interfaces;
using YourBrand.Domain.Entities;

namespace YourBrand.Application.Themes;

public record CopyTheme(string Id) : IRequest<ThemeDto>
{
    public class Handler(IAppServiceContext appServiceContext) : IRequestHandler<CopyTheme, ThemeDto>
    {
        public async Task<ThemeDto?> Handle(CopyTheme request, CancellationToken cancellationToken)
        {
            Theme? theme = await appServiceContext.Themes
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

            theme.SetId(Guid.NewGuid().ToString());
            theme.Name = IncrementCopyName(theme.Name);

            appServiceContext.Themes.Add(theme);

            await appServiceContext.SaveChangesAsync(cancellationToken);

            theme = await appServiceContext.Themes
                .FirstAsync(x => x.Id == request.Id, cancellationToken);

            return theme.ToDto();
        }

        private void Map(ThemeColorScheme target, ThemeColorSchemeDto? from)
        {
            target.BackgroundColor = from.BackgroundColor;
            target.AppbarBackgroundColor = from.AppbarBackgroundColor;
            target.PrimaryColor = from.PrimaryColor;
            target.SecondaryColor = from.SecondaryColor;
        }
    }

    public static string IncrementCopyName(string name)
    {
        var pattern = @"^(.*?)(?: \(Copy (\d+)\))?$";
        var match = Regex.Match(name, pattern);

        if (match.Success)
        {
            string baseName = match.Groups[1].Value.Trim();
            string numberPart = match.Groups[2].Value;

            int newNumber = string.IsNullOrEmpty(numberPart) ? 1 : int.Parse(numberPart) + 1;
            return $"{baseName} (Copy {newNumber})";
        }

        return name;
    }

}