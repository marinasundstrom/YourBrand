using YourBrand.Customers.Domain.Entities;

namespace YourBrand.Customers.Application;

public static class Mappings 
{
    public static PersonDto ToDto(this Person person) 
    {
        return new PersonDto(person.Id, person.FirstName, person.LastName, person.Ssn);
    }

    public static AddressDto ToDto(this Address address) 
    {
        return new AddressDto(address.Id, address.Thoroughfare, address.Premises, 
            address.SubPremises, address.PostalCode, address.Locality, 
            address.SubAdministrativeArea, address.AdministrativeArea, address.Country);
    }
}