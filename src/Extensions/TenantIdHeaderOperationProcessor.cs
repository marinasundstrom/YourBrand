using System.Reflection;

using Microsoft.AspNetCore.Http;

using NJsonSchema;

using NSwag;
using NSwag.Generation.Processors;
using NSwag.Generation.Processors.Contexts;

namespace YourBrand.Extensions;

public class TenantIdHeaderOperationProcessor(bool isRequired) : IOperationProcessor
{
    public bool Process(OperationProcessorContext context)
    {
        context.OperationDescription.Operation.Parameters.Add(
            new OpenApiParameter
            {
                Name = "TenantId",
                Kind = OpenApiParameterKind.Header,
                Schema = new NJsonSchema.JsonSchema { Type = JsonObjectType.String },
                IsRequired = isRequired,
                Description = "The Id of the tenant",
                Default = null
            });

        return true;
    }
}

public class EndpointAttributesProcessor : IOperationProcessor
{
    public bool Process(OperationProcessorContext context)
    {
        context.OperationDescription.Operation.Summary = context.MethodInfo.GetCustomAttributes<EndpointSummaryAttribute>().FirstOrDefault()?.Summary;

        context.OperationDescription.Operation.Description = context.MethodInfo.GetCustomAttributes<EndpointDescriptionAttribute>().FirstOrDefault()?.Description;

        return true;
    }
}