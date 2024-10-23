namespace YourBrand.Catalog.Domain.Entities;

public interface IHasBrand
{
    public Brand? Brand { get; set; }

    public int? BrandId { get; set; }
}