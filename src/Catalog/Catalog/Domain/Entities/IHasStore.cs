namespace YourBrand.Catalog.Domain.Entities;

public interface IHasStore
{
    public Store Store { get; set; }

    public string StoreId { get; set;}
}
