using YourBrand.Warehouse.Domain.Entities;
using YourBrand.Warehouse.Application.Items;

namespace YourBrand.Warehouse.Application;

public static class Mappings 
{
    public static ItemDto ToDto(this Item person) 
    {
        return new ItemDto(person.Id, person.FirstName, person.LastName, person.Ssn, person.PhoneHome, person.PhoneMobile, person.Email);
    }
}