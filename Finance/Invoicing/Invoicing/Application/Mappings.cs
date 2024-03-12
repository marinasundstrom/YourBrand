using YourBrand.Invoicing.Domain.Entities;

namespace YourBrand.Invoicing.Application;

public static class Mappings 
{
    public static InvoiceDto ToDto(this Invoice invoice) 
    {
        return new InvoiceDto(invoice.Id, invoice.InvoiceNo, invoice.IssueDate, invoice.Type, invoice.Status, invoice.DueDate, invoice.Currency, invoice.Reference, invoice.Note, invoice.BillingDetails?.ToDto(), invoice.ShippingDetails?.ToDto(), invoice.Items.Select(i => i.ToDto()), invoice.SubTotal, invoice.VatAmounts.Select(x => new InvoiceVatAmountDto(x.Name, x.VatRate, x.SubTotal, x.Vat, x.Total)), invoice.Vat, invoice.Total, invoice.Paid, invoice.DomesticService?.ToDto());
    }

    public static InvoiceDomesticServiceDto ToDto(this Domain.Entities.InvoiceDomesticService domesticService) 
    {
        return new InvoiceDomesticServiceDto(domesticService.Kind, domesticService.Buyer, domesticService.Description, domesticService.RequestedAmount, domesticService.PropertyDetails?.ToDto());
    }

    public static InvoiceItemDto ToDto(this InvoiceItem item) 
    {
        return new InvoiceItemDto(item.Id, item.ProductType, item.Description, item.Price, item.Unit, item.VatRate.GetValueOrDefault(), item.Quantity, item.Vat.GetValueOrDefault(), item.Total, item.IsTaxDeductibleService, item.DomesticService?.ToDto());
    }

    public static InvoiceItemDomesticServiceDto ToDto(this Domain.Entities.InvoiceItemDomesticService domesticService) 
    {
        return new InvoiceItemDomesticServiceDto(domesticService.Kind, domesticService.HomeRepairAndMaintenanceServiceType, domesticService.HouseholdServiceType);
    }

    public static PropertyDetailsDto ToDto(this Domain.Entities.PropertyDetails propertyDetails) 
    {
        return new PropertyDetailsDto(propertyDetails.Type, propertyDetails.PropertyDesignation, propertyDetails.ApartmentNo, propertyDetails.OrganizationNo);
    }

    public static AddressDto ToDto(this Address address) => new()
    {
        Thoroughfare = address.Thoroughfare,
        Premises = address.Premises,
        SubPremises = address.SubPremises,
        PostalCode = address.PostalCode,
        Locality = address.Locality,
        SubAdministrativeArea = address.SubAdministrativeArea,
        AdministrativeArea = address.AdministrativeArea,
        Country = address.Country
    };

    public static BillingDetailsDto ToDto(this BillingDetails billingDetails) => new()
    {
        FirstName = billingDetails.FirstName,
        LastName = billingDetails.LastName,
        SSN = billingDetails.SSN,
        Email = billingDetails.Email,
        PhoneNumber = billingDetails.PhoneNumber,
        Address = billingDetails.Address?.ToDto()
    };

    public static ShippingDetailsDto ToDto(this ShippingDetails billingDetails) => new()
    {
        FirstName = billingDetails.FirstName,
        LastName = billingDetails.LastName,
        Address = billingDetails.Address?.ToDto()
    };

    //public static CurrencyAmountDto ToDto(this CurrencyAmount currencyAmount) => new(currencyAmount.Currency, currencyAmount.Amount);
}