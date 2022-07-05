namespace YourBrand.Products.Application.Attributes;

public record class AttributeDto(string Id, string Name, string? Description, AttributeGroupDto? Group, IEnumerable<AttributeValueDto> Values);

