using YourBrand.Marketing.Application.Addresses;
using YourBrand.Marketing.Domain.Entities;
using YourBrand.Marketing.Application.Contacts;
using YourBrand.Marketing.Application.Campaigns;
using YourBrand.Marketing.Application.Discounts;

namespace YourBrand.Marketing.Application;

public static class Mappings 
{
    public static ContactDto ToDto(this Contact person) 
    {
        return new ContactDto(person.Id, person.Campaign?.ToDto(), person.FirstName, person.LastName, person.Ssn, person.Phone, person.PhoneMobile, person.Email, person.Address?.ToDto());
    }

    public static AddressDto ToDto(this Address address) 
    {
        return new AddressDto(address.Id, address.Type, address.Thoroughfare, address.Premises, 
            address.SubPremises, address.PostalCode, address.Locality, 
            address.SubAdministrativeArea, address.AdministrativeArea, address.Country);
    }

    public static ContactAddressDto ToDto(this Domain.ValueObjects.Address address) 
    {
        return new ContactAddressDto(address.Thoroughfare, address.Premises, 
            address.SubPremises, address.PostalCode, address.Locality, 
            address.SubAdministrativeArea, address.AdministrativeArea, address.Country);
    }

    public static CampaignDto ToDto(this Campaign campaign) 
    {
        return new CampaignDto(campaign.Id, campaign.Name);
    }

    public static DiscountDto ToDto(this Discount discount) 
    {
        return new DiscountDto(discount.Id, string.Empty);
    }

}