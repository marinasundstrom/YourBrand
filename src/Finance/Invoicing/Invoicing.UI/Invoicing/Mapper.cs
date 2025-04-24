using YourBrand.Invoicing.Client;

namespace YourBrand.Invoicing.Invoicing;

public static class Mapper
{
    public static InvoiceViewModel ToModel(this Invoice dto)
    {
        var model = new InvoiceViewModel
        {
            Id = dto.Id,
            InvoiceNo = dto.InvoiceNo,
            Status = dto.Status,
            Date = dto.IssueDate is null ? null : dto.IssueDate.GetValueOrDefault(),
            DueDate = dto.DueDate is null ? null : dto.DueDate.GetValueOrDefault(),
            OrderNo = (int?)dto.OrderNo,
            Reference = dto.Reference,
            Note = dto.Note,
            Paid = dto.Paid
        };

        foreach (var item in dto.Items)
        {
            model.AddItem(item.ToModel());
        }

        model.DomesticService = dto.DomesticService?.ToModel();

        return model;
    }

    public static InvoiceItemViewModel ToModel(this InvoiceItem dto)
    {
        return new InvoiceItemViewModel
        {
            Id = dto.Id,
            Description = dto.Description,
            ProductType = dto.ProductType,
            ItemId = dto.ProductId,
            UnitPrice = dto.UnitPrice,
            Unit = dto.Unit!,
            Quantity = dto.Quantity,
            //RegularPrice = dto.UnitPrice + dto.Discount,
            VatRate = dto.VatRate.GetValueOrDefault(),
            IsTaxDeductibleService = dto.IsTaxDeductibleService,
            DomesticService = dto.DomesticService is null ?
             new InvoiceItemDomesticServiceViewModel()
             : dto.DomesticService.ToModel()
        };
    }

    public static InvoiceDomesticServiceViewModel ToModel(this InvoiceDomesticService dto)
    {
        return new InvoiceDomesticServiceViewModel
        {
            Kind = dto.Kind,
            Description = dto.Description,
            Buyer = dto.Buyer,
            RequestedAmount = dto.RequestedAmount
        };
    }

    public static InvoiceDomesticService To(this InvoiceDomesticServiceViewModel dto)
    {
        return new InvoiceDomesticService
        {
            Kind = dto.Kind.GetValueOrDefault(),
            Description = dto.Description,
            Buyer = dto.Buyer,
            RequestedAmount = dto.RequestedAmount.GetValueOrDefault()
        };
    }

    public static CreateInvoiceItem ToAddInvoiceItem(this InvoiceItemViewModel vm)
    {
        return new CreateInvoiceItem
        {
            Description = vm.Description,
            ProductType = vm.ProductType,
            ProductId = vm.ItemId,
            UnitPrice = vm.UnitPrice,
            Unit = vm.Unit,
            Quantity = vm.Quantity,
            //Discount = vm.Discount,
            VatRate = vm.VatRate,
            IsTaxDeductibleService = vm.IsTaxDeductibleService,
            DomesticService = !vm.IsTaxDeductibleService ? null
             : vm.DomesticService?.To()
        };
    }

    public static InvoiceItemDomesticServiceViewModel ToModel(this InvoiceItemDomesticService dto)
    {
        return new InvoiceItemDomesticServiceViewModel()
        {
            Kind = dto.Kind,
            HomeRepairAndMaintenanceServiceType = dto.HomeRepairAndMaintenanceServiceType,
            HouseholdServiceType = dto.HouseholdServiceType
        };
    }
    public static InvoiceItemDomesticService To(this InvoiceItemDomesticServiceViewModel vm)
    {
        return new InvoiceItemDomesticService()
        {
            Kind = (DomesticServiceKind)vm.Kind.GetValueOrDefault(),
            HomeRepairAndMaintenanceServiceType = vm.HomeRepairAndMaintenanceServiceType,
            HouseholdServiceType = vm.HouseholdServiceType
        };
    }

    public static UpdateInvoiceItem ToUpdateInvoiceItem(this InvoiceItemViewModel dto)
    {
        return new UpdateInvoiceItem
        {
            Description = dto.Description,
            ProductType = dto.ProductType,
            ProductId = dto.ItemId,
            UnitPrice = dto.UnitPrice,
            Unit = dto.Unit,
            Quantity = dto.Quantity,
            Discount = dto.Discount,
            VatRate = dto.VatRate,
            IsTaxDeductibleService = dto.IsTaxDeductibleService,
            /*DomesticService = dto.DomesticService is null ? null
            : new InvoiceItemDomesticService()
            {
                Kind = (DomesticServiceKind)dto.DomesticService.Kind,
                HomeRepairAndMaintenanceServiceType = dto.DomesticService.HomeRepairAndMaintenanceServiceType,
                HouseholdServiceType = dto.DomesticService.HouseholdServiceType
            }
          */
        };
    }
}