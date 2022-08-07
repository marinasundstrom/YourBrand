namespace YourBrand.Catalog.Application.Attributes;

public static class Mapping
{
    public static AttributeDto ToDto(this Domain.Entities.Attribute x)
    {
        return new AttributeDto(x.Id, x.Name, x.Description, x.Group == null ? null : new AttributeGroupDto(x.Group.Id, x.Group.Name, x.Group.Description), x.ForVariant, x.IsMainAttribute,
                x.Values.Select(x => new AttributeValueDto(x.Id, x.Name, x.Seq)));
    }
}
