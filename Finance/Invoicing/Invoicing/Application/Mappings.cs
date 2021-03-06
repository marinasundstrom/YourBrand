using YourBrand.Invoicing.Domain.Entities;

namespace YourBrand.Invoicing.Application;

public static class Mappings 
{
    public static InvoiceDto ToDto(this Invoice invoice) 
    {
        return new InvoiceDto(invoice.Id, invoice.Date, invoice.Type, invoice.Status, invoice.DueDate, invoice.Currency, invoice.Reference, invoice.Note, invoice.Items.Select(i => i.ToDto()),   invoice.SubTotal, invoice.Vat, invoice.Total, invoice.Paid, invoice.DomesticService?.ToDto());
    }

    public static InvoiceDomesticServiceDto ToDto(this Domain.Entities.InvoiceDomesticService domesticService) 
    {
        return new InvoiceDomesticServiceDto(domesticService.Kind, domesticService.Buyer, domesticService.Description, domesticService.RequestedAmount, domesticService.PropertyDetails?.ToDto());
    }

    public static InvoiceItemDto ToDto(this InvoiceItem item) 
    {
        return new InvoiceItemDto(item.Id, item.ProductType, item.Description, item.UnitPrice, item.Unit, item.VatRate, item.Quantity, item.LineTotal, item.IsTaxDeductibleService, item.DomesticService?.ToDto());
    }

    public static InvoiceItemDomesticServiceDto ToDto(this Domain.Entities.InvoiceItemDomesticService domesticService) 
    {
        return new InvoiceItemDomesticServiceDto(domesticService.Kind, domesticService.HomeRepairAndMaintenanceServiceType, domesticService.HouseholdServiceType);
    }

    public static PropertyDetailsDto ToDto(this Domain.Entities.PropertyDetails propertyDetails) 
    {
        return new PropertyDetailsDto(propertyDetails.Type, propertyDetails.PropertyDesignation, propertyDetails.ApartmentNo, propertyDetails.OrganizationNo);
    }
}