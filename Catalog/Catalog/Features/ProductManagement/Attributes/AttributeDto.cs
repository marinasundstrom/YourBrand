namespace YourBrand.Catalog.Features.ProductManagement.Attributes;

public record class AttributeDto(string Id, string Name, string? Description, AttributeGroupDto? Group, IEnumerable<AttributeValueDto> Values);

public record class Attribute2Dto(string Id, string Name, string? Description, AttributeGroupDto? Group);