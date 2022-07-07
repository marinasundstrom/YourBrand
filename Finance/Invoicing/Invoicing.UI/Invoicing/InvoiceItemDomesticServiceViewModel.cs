using YourBrand.Invoicing.Client;

namespace YourBrand.Invoicing.Invoicing;

public class InvoiceItemDomesticServiceViewModel
{
    public DomesticServiceKind? Kind { get; set; }

    public HomeRepairAndMaintenanceServiceType? HomeRepairAndMaintenanceServiceType { get; set; }

    public HouseholdServiceType? HouseholdServiceType { get; set; }
}