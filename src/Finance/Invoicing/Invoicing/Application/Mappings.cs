using Microsoft.EntityFrameworkCore;

using YourBrand.Invoicing.Domain.Entities;
using YourBrand.Invoicing.Infrastructure.Persistence;

namespace YourBrand.Invoicing.Application;

public static class Mappings
{
    public static InvoiceDto ToDto(this Invoice invoice)
    {
        return new InvoiceDto(
            invoice.Id,
            invoice.OrganizationId,
            invoice.InvoiceNo,
            invoice.IssueDate,
            invoice.Type,
            invoice.Status.ToDto(),
            invoice.StatusDate,
            invoice.Customer?.ToDto(),
            invoice.DueDate,
            invoice.Currency,
            invoice.Reference,
            invoice.Notes,
            invoice.BillingDetails?.ToDto(),
            invoice.ShippingDetails?.ToDto(),
            invoice.Items.Select(i => i.ToDto()),
            invoice.SubTotal,
            invoice.VatAmounts.Select(x => new InvoiceVatAmountDto(x.Name, x.VatRate, x.SubTotal, x.Vat, x.Total)),
            invoice.VatRate,
            invoice.Vat,
            invoice.Discount,
            invoice.Total,
            invoice.Paid,
            invoice.DomesticService?.ToDto(),
            invoice.Created,
            invoice.CreatedById,
            invoice.LastModified,
            invoice.LastModifiedById);
    }

    public static InvoiceDomesticServiceDto ToDto(this Domain.Entities.InvoiceDomesticService domesticService)
    {
        return new InvoiceDomesticServiceDto(domesticService.Kind, domesticService.Buyer, domesticService.Description, domesticService.RequestedAmount, domesticService.PropertyDetails?.ToDto());
    }

    public static InvoiceItemDto ToDto(this InvoiceItem item)
    {
        return new InvoiceItemDto(
            item.Id,
            item.ProductType,
            item.Description,
            item.ProductId,
            item.Sku,
            item.Price,
            item.Unit,
            item.Discount,
            item.RegularPrice,
            item.VatRate.GetValueOrDefault(),
            item.Quantity,
            item.Vat.GetValueOrDefault(),
            item.Total,
            item.Notes,
            item.IsTaxDeductibleService,
            item.DomesticService?.ToDto(),
            item.Created,
            item.CreatedById,
            item.LastModified,
            item.LastModifiedById);
    }

    public static CustomerDto ToDto(this Customer customer) => new CustomerDto(customer.Id, customer.CustomerNo, customer.Name);

    public static InvoiceStatusDto ToDto(this InvoiceStatus status) => new InvoiceStatusDto(status.Id, status.OrganizationId, status.Name);

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

public sealed class InvoiceNumberFetcher(InvoicingContext invoicingContext)
{
    public async Task<int> GetNextNumberAsync(string organizationId, CancellationToken cancellationToken)
    {
        int invoiceNo;

        try
        {
            invoiceNo = (await invoicingContext.Invoices
                .IgnoreQueryFilters()
                .InOrganization(organizationId)
                .MaxAsync(x => x.InvoiceNo.GetValueOrDefault(), cancellationToken: cancellationToken)) + 1;
        }
        catch (InvalidOperationException e)
        {
            invoiceNo = 1; // Invoice start number
        }

        return invoiceNo;
    }
}