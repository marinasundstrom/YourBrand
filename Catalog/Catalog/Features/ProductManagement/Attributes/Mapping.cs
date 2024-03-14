namespace YourBrand.Catalog.Features.ProductManagement.Attributes;

public static class Mapping
{
    public static AttributeDto ToDto(this Domain.Entities.Attribute x)
    {
        return new AttributeDto(x.Id, x.Name, x.Description, x.Group == null ? null : new AttributeGroupDto(x.Group.Id, x.Group.Name, x.Group.Description),
                x.Values.Select(x => x.ToDto()));
    }

    public static Attribute2Dto ToDto2(this Domain.Entities.Attribute x)
    {
        return new Attribute2Dto(x.Id, x.Name, x.Description, x.Group == null ? null : new AttributeGroupDto(x.Group.Id, x.Group.Name, x.Group.Description));
    }

    public static AttributeValueDto ToDto(this Domain.Entities.AttributeValue x)
    {
        return new AttributeValueDto(x.Id, x.Name, x.Seq);
    }
}