namespace Microsoft.Extensions.Hosting;

public static class TypeExtensions 
{
    public static string GetSchemaName(this Type type)
    {
        if (type.IsGenericType)
        {
            // Process the generic arguments
            var typeNames = type.GetGenericArguments().Select(GetSchemaNameCore).ToArray();
            var args = string.Join("And", typeNames);

            // Get the name of the generic type without the arity (backtick and number)
            var typeName = type.Name;
            var index = typeName.IndexOf('`');
            if (index >= 0)
            {
                typeName = typeName.Substring(0, index);
            }

            return $"{typeName}Of{args}";
        }
        return GetSchemaNameCore(type);
    }

    static string GetSchemaNameCore(Type type)
    {
        var typeName = type.Name;
        var suffixes = new[] { "Dto" }; // Add more suffixes if needed

        foreach (var suffix in suffixes)
        {
            if (typeName.EndsWith(suffix, StringComparison.OrdinalIgnoreCase))
            {
                // Ensure the type name is longer than the suffix
                if (typeName.Length > suffix.Length)
                {
                    return typeName.Substring(0, typeName.Length - suffix.Length);
                }
                else
                {
                    return typeName; // Return original name if it's shorter than the suffix
                }
            }
        }

        return type.Name;
    }
}