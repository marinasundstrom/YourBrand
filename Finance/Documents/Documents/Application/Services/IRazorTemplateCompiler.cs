
namespace Documents.Application.Services
{
    public interface IRazorTemplateCompiler
    {
        Task<string> RenderAsync(string templateKey, object model);

        Task<string> CompileAndRenderAsync(string templateKey, string template, object model);

        bool HasCachedTemplate(string templateKey);

        bool RemoveTemplateFromCache(string templateKey);
    }
}