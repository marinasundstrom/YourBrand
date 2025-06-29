using System.Reflection;

using MassTransit.Internals;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.OpenApi;

namespace Microsoft.Extensions.Hosting;

/*
public class NullabilityTransformer : IOpenApiOperationTransformer
{
    public Task TransformAsync(OpenApiOperation operation, OpenApiOperationTransformerContext context, CancellationToken cancellationToken)
    {
        var methodInfo = (context.Description.ActionDescriptor as ControllerActionDescriptor)!.MethodInfo;

        /*
        var x = context.Description.ActionDescriptor.EndpointMetadata
                         .OfType<MethodInfo>()
                         .FirstOrDefault(); *

        //context.Description.ParameterDescriptions.First(x => x.Name == )

        TransformParameters(operation, context, methodInfo);

        //TransformRequestBody(operation, context, methodInfo);

        TransformResponseTypes(operation, context, methodInfo);

        return Task.CompletedTask;
    }

    private static void TransformParameters(OpenApiOperation operation, OpenApiOperationTransformerContext context, MethodInfo methodInfo)
    {
        foreach (var parameterInfo in methodInfo.GetParameters())
        {
            var parameter = operation.Parameters.FirstOrDefault(x => x.Name == parameterInfo.Name);

            if (parameter is null) continue;

            if (parameterInfo.HasDefaultValue)
            {
                IOpenApiAny? def;

                if (parameterInfo.DefaultValue is string str)
                {
                    def = new OpenApiString(str);
                }
                else if (parameterInfo.DefaultValue is int i)
                {
                    def = new OpenApiInteger(i);
                }
                else if (parameterInfo.DefaultValue is double d)
                {
                    def = new OpenApiDouble(d);
                }
                else if (parameterInfo.DefaultValue is bool b)
                {
                    def = new OpenApiBoolean(b);
                }
                else if (parameterInfo.DefaultValue is null)
                {
                    def = null;
                }
                else
                {
                    throw new InvalidOperationException($"Invalid value of type {parameterInfo.DefaultValue?.GetType()}");
                }

                parameter.Schema.Default = def;
            }
            else
            {
                parameter.Required = true;
            }

            var nullabilityContext = new NullabilityInfoContext();
            var nullabilityInfo = nullabilityContext.Create(parameterInfo);

            var isNullable = parameterInfo.ParameterType.IsNullable(out var nt) || (nullabilityInfo.ReadState == NullabilityState.Nullable);

            if (isNullable)
            {
                parameter.Schema.Nullable = isNullable;
            }
        }
    }

    private static void TransformRequestBody(OpenApiOperation operation, OpenApiOperationTransformerContext context, MethodInfo methodInfo)
    {
        /*
        var nullabilityContext = new NullabilityInfoContext();
        var nullabilityInfo = nullabilityContext.Create(param);

        foreach (var (key, request) in operation.RequestBody.Content)
        {
            var isNullable = nullabilityInfo.ReadState == NullabilityState.Nullable;

            if (isNullable)
            {
                var originalSchema = request.Schema;

                var newSchema = new OpenApiSchema();
                request.Schema = newSchema;

                newSchema.AllOf.Add(originalSchema);

                newSchema.Nullable = isNullable;
            }
        } *
    }

    private static void TransformResponseTypes(OpenApiOperation operation, OpenApiOperationTransformerContext context, MethodInfo methodInfo)
    {
        var returnParameter = methodInfo.ReturnParameter;

        var nullabilityContext = new NullabilityInfoContext();
        var nullabilityInfo = nullabilityContext.Create(returnParameter);

        foreach (var (key, response) in operation.Responses)
        {
            foreach (var (key2, content) in response.Content)
            {
                var isNullable = returnParameter.ParameterType.IsNullable(out var nt) || (nullabilityInfo.ReadState == NullabilityState.Nullable);

                if (isNullable)
                {
                    var originalSchema = content.Schema;

                    var newSchema = new OpenApiSchema();
                    content.Schema = newSchema;

                    newSchema.AllOf.Add(originalSchema);

                    newSchema.Nullable = isNullable;
                }
            }
        }
    }

    private static Type UnwrapActionResult(Type returnType, ref NullabilityInfo nullabilityInfo)
{
    if (returnType.IsGenericType)
    {
        var genericDefinition = returnType.GetGenericTypeDefinition();

        // Check for ActionResult<T>
        if (genericDefinition == typeof(ActionResult<>))
        {
            returnType = returnType.GetGenericArguments()[0];
            nullabilityInfo = nullabilityInfo.GenericTypeArguments[0];
        }
    }

    return returnType;
}
}
*/