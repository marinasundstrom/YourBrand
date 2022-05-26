using RazorLight;

namespace Documents.Application.Services;

public sealed class RazorTemplateCompiler : IRazorTemplateCompiler
{
    private readonly RazorLightEngine engine;

    public RazorTemplateCompiler()
    {
        engine = new RazorLightEngineBuilder()
        // required to have a default RazorLightProject type,
        // but not required to create a template from string.
        .UseEmbeddedResourcesProject(typeof(RazorTemplateCompiler))
        .SetOperatingAssembly(typeof(RazorTemplateCompiler).Assembly)
        .UseMemoryCachingProvider()
        .Build();
    }

    public async Task<string> RenderAsync(string templateKey, object model)
    {
        var cacheResult = engine.Handler.Cache.RetrieveTemplate(templateKey);

        if (cacheResult.Success)
        {
            var templatePage = cacheResult.Template.TemplatePageFactory();
            return await engine.RenderTemplateAsync(templatePage, model);
        }
        else
        {
            throw new Exception();
        }
    }

    public async Task<string> CompileAndRenderAsync(string templateKey, string template, object model)
    {
        //string template = "Hello, @Model.Name. Welcome to RazorLight repository";
        //ViewModel model = new ViewModel { Name = "John Doe" };

        return await engine.CompileRenderStringAsync(templateKey, template, model);
    }

    public bool HasCachedTemplate(string templateKey)
    {
        return engine.Handler.Cache.Contains(templateKey);
    }

    public bool RemoveTemplateFromCache(string templateKey)
    {
        var cache = engine.Handler.Cache;

        if (cache.Contains(templateKey))
        {
            cache.Remove(templateKey);
            return true;
        }

        return false;
    }
}