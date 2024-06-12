using NJsonSchema.Generation;

namespace ChatApp.Web;

internal sealed class CustomSchemaNameGenerator : ISchemaNameGenerator
{
    public string Generate(Type type)
    {
        if (type.IsGenericType)
        {
            return $"{type.Name.Replace("`1", string.Empty)}Of{GenerateName(type.GetGenericArguments().First())}";
        }

        return GenerateName(type);
    }

    private static string GenerateName(Type type)
    {
        return type.Name
            .Replace("Dto", string.Empty);
    }
}