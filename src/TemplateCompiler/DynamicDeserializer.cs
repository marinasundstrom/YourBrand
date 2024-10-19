using System.Reflection;
using System.Text.Json;

namespace TemplateCompiler;

public class DynamicDeserializer
{
    public static object DeserializeJsonToType(string json, Type targetType)
    {
        // Step 1: Get the generic method definition
        MethodInfo deserializeMethod = typeof(JsonSerializer).GetMethods()
            .FirstOrDefault(m => m.Name == "Deserialize"
                && m.IsGenericMethod
                && m.GetParameters().Length == 1
                && m.GetParameters()[0].ParameterType == typeof(string));

        if (deserializeMethod == null)
        {
            throw new InvalidOperationException("Could not find Deserialize<T>(string) method.");
        }

        // Step 2: Make a generic method with the specific type argument
        MethodInfo genericMethod = deserializeMethod.MakeGenericMethod(targetType);

        // Step 3: Invoke the method
        object result = genericMethod.Invoke(null, new object[] { json });

        return result;
    }
}