using YourBrand.Invoicing.Client;
using YourBrand.RotRutService.Domain.Entities;

namespace YourBrand.RotRutService.Domain;

public class RotRutCaseFactory
{
    public RotRutCase CreateRotRutCase(Invoice invoice)
    {
        var domesticServices = invoice.DomesticService;

        var itemsWithoutService = invoice.Items.Where(x => x.ProductType == ProductType.Good && !x.IsTaxDeductibleService);
        var services = invoice.Items.Where(x => x.ProductType == ProductType.Service && x.IsTaxDeductibleService);
        var goodsForService = invoice.Items.Where(x => x.ProductType == ProductType.Good && x.IsTaxDeductibleService);

        var hours = services.Sum(x => x.Quantity);
        var laborCost = services.Sum(x => x.Total);
        var materialCost = goodsForService.Sum(x => x.Total);

        decimal maxDeductibleAmount = 0;

        switch (domesticServices.Kind)
        {
            case DomesticServiceKind.HomeRepairAndMaintenanceServiceType:
                maxDeductibleAmount = laborCost.GetRot();
                break;

            case DomesticServiceKind.HouseholdService:
                maxDeductibleAmount = laborCost.GetRut();
                break;
        }

        decimal requestedAmount = domesticServices.RequestedAmount;

        DateTime paymentDate = DateTime.Now; // TODO: Add payment date to invoice
        decimal paidAmount = invoice.Total;
        decimal otherCosts = itemsWithoutService.Sum(x => x.Total);

        if (requestedAmount < 1)
        {
            throw new Exception("No deductible");
        }

        if (requestedAmount > maxDeductibleAmount)
        {
            throw new Exception("Exceeds maximum deductible amount");
        }

        var rotRutCase =
            new Domain.Entities.RotRutCase(
                (Domain.Enums.DomesticServiceKind)domesticServices.Kind,
                invoice.DomesticService!.Buyer, paymentDate, laborCost,
                paidAmount, requestedAmount, int.Parse(invoice.InvoiceNo), otherCosts, hours, materialCost, null);

        if (domesticServices.Kind == DomesticServiceKind.HomeRepairAndMaintenanceServiceType)
        {
            var first = services.First();

            rotRutCase.Rot = new Domain.Entities.Rot()
            {
                ServiceType = (Domain.Enums.HomeRepairAndMaintenanceServiceType?)first.DomesticService!.HomeRepairAndMaintenanceServiceType,
                PropertyDesignation = invoice.DomesticService!.PropertyDetails!.PropertyDesignation,
                ApartmentNo = invoice.DomesticService!.PropertyDetails!.ApartmentNo,
                OrganizationNo = invoice.DomesticService!.PropertyDetails!.OrganizationNo
            };
        }
        else if (domesticServices.Kind == DomesticServiceKind.HouseholdService)
        {
            var first = services.First();

            rotRutCase.Rut = new Domain.Entities.Rut()
            {
                ServiceType = (Domain.Enums.HouseholdServiceType?)first.DomesticService!.HouseholdServiceType
            };
        }

        return rotRutCase;
    }
}