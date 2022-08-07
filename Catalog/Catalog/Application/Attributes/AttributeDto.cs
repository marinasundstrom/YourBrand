namespace YourBrand.Catalog.Application.Attributes;

public record class AttributeDto(string Id, string Name, string? Description, AttributeGroupDto? Group, bool ForVariant, bool IsMainAttribute, IEnumerable<AttributeValueDto> Values);

