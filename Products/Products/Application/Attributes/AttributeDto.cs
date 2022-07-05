namespace YourBrand.Products.Application.Attributes;

public record class AttributeDto(string Id, string Name, string? Description, AttributeGroupDto? Group, bool ForVariant, IEnumerable<AttributeValueDto> Values);

