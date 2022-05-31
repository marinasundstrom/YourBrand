using YourBrand.Customers.Application.Addresses;
using YourBrand.Customers.Domain.Entities;
using YourBrand.Customers.Application.Persons;

namespace YourBrand.Customers.Application;

public static class Mappings 
{
    public static PersonDto ToDto(this Person person) 
    {
        return new PersonDto(person.Id, person.FirstName, person.LastName, person.Ssn, person.PhoneHome, person.PhoneMobile, person.Email, person.Addresses.Select(a => a.ToDto()));
    }

    public static AddressDto ToDto(this Address address) 
    {
        return new AddressDto(address.Id, address.Type, address.Thoroughfare, address.Premises, 
            address.SubPremises, address.PostalCode, address.Locality, 
            address.SubAdministrativeArea, address.AdministrativeArea, address.Country);
    }
}