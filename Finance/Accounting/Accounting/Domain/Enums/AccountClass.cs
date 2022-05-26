using System.ComponentModel.DataAnnotations;

namespace Accounting.Domain.Enums;

public enum AccountClass
{
    [Display(Name = "Assets")]
    Assets = 1,

    [Display(Name = "Equity and liabilities")]
    EquityAndLiabilites = 2,

    [Display(Name = "Operating income/revenue")]
    OperatingIncomeRevenue = 3,

    [Display(Name = "Cost of goods, materials and certain sub-contract work")]
    Costs = 4,

    [Display(Name = "Other external operating expenses/costs (5)")]
    OtherOperatingExpenses1 = 5,

    [Display(Name = "Other external operating expenses/costs (6)")]
    OtherOperatingExpenses2 = 6,

    [Display(Name = "Personnel costs, depreciation etc.")]
    PersonnelCosts = 7,

    [Display(Name = "Financial and other income and expenses")]
    FinancialAndOtherIncomeAndExpenses = 8
}