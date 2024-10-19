using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Infrastructure;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.RenderTree;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Razor.Language;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace TemplateCompiler;

public class DynamicRazorComponentRenderer
{
    private readonly IMemoryCache _cache;
    private readonly HtmlRenderer _renderer;
    private readonly ILogger<DynamicRazorComponentRenderer> _logger;

    public DynamicRazorComponentRenderer(IMemoryCache cache, HtmlRenderer renderer, ILogger<DynamicRazorComponentRenderer> logger)
    {
        _cache = cache;
        _renderer = renderer;
        _logger = logger;
        
        AssemblyReferences.AddRange(new[]
        {
            typeof(object).Assembly,
            typeof(ComponentBase).Assembly,
            typeof(RenderFragment).Assembly,
            typeof(Enumerable).Assembly,
            typeof(Task).Assembly,
            // Add other necessary types
        });
    }

    public List<Assembly> AssemblyReferences { get; set; } = new List<Assembly>();

    public async Task<string> RenderComponentAsync(string componentKey, string componentContent, object model)
    {
        // Try to get the component type from the cache
        if (!_cache.TryGetValue(componentKey, out Type componentType))
        {
            // Compile the component and cache the type
            componentType = CompileRazorComponent(componentContent);
            if (componentType == null)
            {
                throw new Exception("Component compilation failed.");
            }

            // Set cache options (e.g., sliding expiration)
            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromHours(1));

            _cache.Set(componentKey, componentType, cacheEntryOptions);
        }

        // Prepare parameters
        var parameters = new Dictionary<string, object?>
        {
            { "Model", model }
        };

        var parameterView = ParameterView.FromDictionary(parameters);

        // Render the component to HTML
        var htmlRootComponent = await _renderer.Dispatcher.InvokeAsync(() =>
            _renderer.RenderComponentAsync(componentType, parameterView)
        );

        return htmlRootComponent.ToHtmlString();
    }

    private Type CompileRazorComponent(string componentContent)
    {
        // Create the in-memory project item
        var projectItem = new InMemoryRazorProjectItem("/", "/DynamicComponent.razor", componentContent);

        // Create the project engine with an empty file system
        var projectEngine = RazorProjectEngine.Create(RazorConfiguration.Default, RazorProjectFileSystem.Create("."), builder =>
        {
            // Set the namespace for the generated component
            builder.SetNamespace("DynamicComponentsNamespace");
        });

        // Process the project item
        var codeDocument = projectEngine.Process(projectItem);
        var csharpDocument = codeDocument.GetCSharpDocument();
        var generatedCode = csharpDocument.GeneratedCode;

        // Compile the generated C# code into an assembly
        var syntaxTree = CSharpSyntaxTree.ParseText(generatedCode);

        // Reference necessary assemblies
        var references = GetMetadataReferences();

        var compilation = CSharpCompilation.Create(
            assemblyName: $"DynamicComponentAssembly_{Guid.NewGuid()}",
            syntaxTrees: new[] { syntaxTree },
            references: references,
            options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        using var ms = new MemoryStream();
        var result = compilation.Emit(ms);

        if (!result.Success)
        {
            var failures = result.Diagnostics.Where(diagnostic =>
                diagnostic.IsWarningAsError || diagnostic.Severity == DiagnosticSeverity.Error);

            foreach (var diagnostic in failures)
            {
                _logger.LogError(diagnostic.ToString());
            }

            throw new InvalidOperationException("Component compilation failed.");
        }

        ms.Seek(0, SeekOrigin.Begin);
        var assembly = Assembly.Load(ms.ToArray());

        // Get the component type
        var componentTypeName = GetComponentTypeName(csharpDocument);
        var componentType = assembly.GetType(componentTypeName);

        return componentType;
    }

    private List<PortableExecutableReference> GetMetadataReferences()
    {
        var assemblies = AppDomain.CurrentDomain.GetAssemblies()
            .Where(a => !a.IsDynamic && !string.IsNullOrWhiteSpace(a.Location))
            .ToArray();

        var references = assemblies.Select(a => MetadataReference.CreateFromFile(a.Location)).ToList();

        // Ensure necessary assemblies are included
        var necessaryAssemblies = AssemblyReferences;

        var existingLocations = new HashSet<string?>(references.Select(r => r.FilePath), StringComparer.OrdinalIgnoreCase);

        foreach (var assembly in necessaryAssemblies)
        {
            if (!existingLocations.Contains(assembly.Location))
            {
                var reference = MetadataReference.CreateFromFile(assembly.Location);
                references.Add(reference);
                existingLocations.Add(assembly.Location);
            }
        }
        return references;
    }

    private string GetComponentTypeName(RazorCSharpDocument csharpDocument)
    {
        var code = csharpDocument.GeneratedCode;
        string namespaceLine = null;
        string classLine = null;

        using (var reader = new StringReader(code))
        {
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                line = line.Trim();
                if (line.StartsWith("namespace "))
                {
                    namespaceLine = line;
                }
                else if (line.StartsWith("public partial class "))
                {
                    classLine = line;
                    break;
                }
            }
        }

        if (classLine != null)
        {
            var @namespace = namespaceLine?.Substring("namespace ".Length).Trim();
            var className = classLine.Substring("public partial class ".Length).Split(' ')[0];

            if (!string.IsNullOrEmpty(@namespace))
            {
                return $"{@namespace}.{className}";
            }
            else
            {
                return className;
            }
        }

        // Default if parsing fails
        return "DynamicComponent";
    }
}

public class InMemoryRazorProjectItem : RazorProjectItem
{
    private readonly string _content;

    public InMemoryRazorProjectItem(string basePath, string filePath, string content)
    {
        BasePath = basePath;
        FilePath = filePath;
        PhysicalPath = null; // No physical path since it's in-memory
        _content = content;
    }

    public override string BasePath { get; }

    public override string FilePath { get; }

    public override string PhysicalPath { get; }

    public override bool Exists => true;

    public override Stream Read()
    {
        return new MemoryStream(Encoding.UTF8.GetBytes(_content));
    }
}