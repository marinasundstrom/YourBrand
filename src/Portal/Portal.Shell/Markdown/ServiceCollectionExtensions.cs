using Ganss.Xss;

using Microsoft.Extensions.DependencyInjection;

namespace YourBrand.Portal.Markdown;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Add Markdown services.
    /// </summary>
    /// <param name="services"></param>
    public static void AddMarkdownServices(this IServiceCollection services)
    {
        services.AddScoped<IHtmlSanitizer, HtmlSanitizer>(x =>
        {
            // Configure sanitizer rules as needed here.
            // For now, just use default rules + allow class attributes
            var sanitizer = new Ganss.Xss.HtmlSanitizer();
            sanitizer.AllowedAttributes.Add("class");
            return sanitizer;
        });
    }
}