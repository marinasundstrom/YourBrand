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