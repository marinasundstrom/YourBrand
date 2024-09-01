using YourBrand.Accounting.Domain.Enums;
using YourBrand.Domain;

namespace YourBrand.Accounting.Domain.Entities;

public static class Accounts
{
    static IEnumerable<Account>? accounts;

    public static IEnumerable<Account> GetAll(OrganizationId organizationId)
    {
        return accounts ??= new[]
        {
            new Account
            {
                AccountNo = 1510,
                OrganizationId = organizationId,
                Class = AccountClass.Assets,
                Name = "Kundfordringar",
                Description = String.Empty
            },
            new Account
            {
                AccountNo = 1513,
                OrganizationId = organizationId,
                Class = AccountClass.Assets,
                Name = "Kundfordringar - delad faktura",
                Description = "För ROT/RUT-avdrag"
            },
            new Account
            {
                AccountNo = 1930,
                OrganizationId = organizationId,
                Class = AccountClass.Assets,
                Name = "Företagskonto",
                Description = String.Empty
            },
            new Account
            {
                AccountNo = 1910,
                OrganizationId = organizationId,
                Class = AccountClass.Assets,
                Name = "Kassa",
                Description = "Insättningar av kontanter från kassan"
            },
            new Account
            {
                AccountNo = 1920,
                OrganizationId = organizationId,
                Class = AccountClass.Assets,
                Name = "PlusGiro",
                Description = String.Empty
            },
            new Account
            {
                AccountNo = 1630,
                OrganizationId = organizationId,
                Class = AccountClass.Assets,
                Name = "Skattekonto",
                Description = String.Empty
            },
            new Account
            {
                AccountNo = 1940,
                OrganizationId = organizationId,
                Class = AccountClass.Assets,
                Name = "Placeringskonto",
                Description = "Överföringar till och från placeringskonto/sparkonto"
            },
            new Account
            {
                AccountNo = 2010,
                OrganizationId = organizationId,
                Class = AccountClass.EquityAndLiabilites,
                Name = "Eget kapital",
                Description = String.Empty,
            },
            new Account
            {
                AccountNo = 2011,
                OrganizationId = organizationId,
                Class = AccountClass.EquityAndLiabilites,
                Name = "Egna varuttag",
                Description = String.Empty,
            },
            new Account
            {
                AccountNo = 2013,
                OrganizationId = organizationId,
                Class = AccountClass.EquityAndLiabilites,
                Name = "Egna Uttag",
                Description = "Privata uttag \"lön\" i enskild firma"
            },
            new Account
            {
                AccountNo = 2018,
                OrganizationId = organizationId,
                Class = AccountClass.EquityAndLiabilites,
                Name = "Egna Insättningar",
                Description = "Privata insättningar i enskild firma"
            },
            new Account
            {
                AccountNo = 2068,
                OrganizationId = organizationId,
                Class = AccountClass.EquityAndLiabilites,
                Name = "Vinst eller förlust från föregående år",
                Description = String.Empty,
            },
            new Account
            {
                AccountNo = 2330,
                OrganizationId = organizationId,
                Class = AccountClass.EquityAndLiabilites,
                Name = "Checkkredit",
                Description = String.Empty,
            },
            new Account
            {
                AccountNo = 2411,
                OrganizationId = organizationId,
                Class = AccountClass.EquityAndLiabilites,
                Name = "Kortfristiga lån från kreditinstitut",
                Description = String.Empty
            },
            new Account
            {
                AccountNo = 2420,
                OrganizationId = organizationId,
                Class = AccountClass.EquityAndLiabilites,
                Name = "Förskott från kunder",
                Description = String.Empty
            },
            new Account
            {
                AccountNo = 2440,
                OrganizationId = organizationId,
                Class = AccountClass.EquityAndLiabilites,
                Name = "Leverantörsskulder",
                Description = String.Empty
            },
            new Account
            {
                AccountNo = 2455,
                OrganizationId = organizationId,
                Class = AccountClass.EquityAndLiabilites,
                Name = "Kortfristiga lån i utländsk valuta",
                Description = String.Empty
            },
            new Account
            {
                AccountNo = 2510,
                OrganizationId = organizationId,
                Class = AccountClass.EquityAndLiabilites,
                Name = "Skatteskulder",
                Description = String.Empty,
            },
            new Account
            {
                AccountNo = 2610,
                OrganizationId = organizationId,
                Class = AccountClass.EquityAndLiabilites,
                Name = "Utgående moms",
                Description = String.Empty
            },
            new Account
            {
                AccountNo = 2611,
                OrganizationId = organizationId,
                Class = AccountClass.EquityAndLiabilites,
                Name = "Utgående moms Försäljning Sverige 25%",
                Description = String.Empty
            },
            new Account
            {
                AccountNo = 2620,
                OrganizationId = organizationId,
                Class = AccountClass.EquityAndLiabilites,
                Name = "Utgående moms 12%",
                Description = String.Empty
            },
            new Account
            {
                AccountNo = 2621,
                OrganizationId = organizationId,
                Class = AccountClass.EquityAndLiabilites,
                Name = "Utgående moms Försäljning Sverige 12%",
                Description = String.Empty
            },
            new Account
            {
                AccountNo = 2630,
                OrganizationId = organizationId,
                Class = AccountClass.EquityAndLiabilites,
                Name = "Utgående moms 6%",
                Description = String.Empty
            },
            new Account
            {
                AccountNo = 2631,
                OrganizationId = organizationId,
                Class = AccountClass.EquityAndLiabilites,
                Name = "Utgående moms Försäljning Sverige 6%",
                Description = String.Empty
            },
            new Account
            {
                AccountNo = 2640,
                OrganizationId = organizationId,
                Class = AccountClass.EquityAndLiabilites,
                Name = "Ingående moms",
                Description = String.Empty
            },
            new Account
            {
                AccountNo = 2650,
                OrganizationId = organizationId,
                Class = AccountClass.EquityAndLiabilites,
                Name = "Redovisningskonto för moms",
                Description = "Får tillbaka eller betalar moms till skatteverket"
            },
            new Account
            {
                AccountNo = 2731,
                OrganizationId = organizationId,
                Class = AccountClass.EquityAndLiabilites,
                Name = "Avräkning arbetsgivaravgift",
                Description = string.Empty
            },
            new Account
            {
                AccountNo = 3000,
                OrganizationId = organizationId,
                Class = AccountClass.OperatingIncomeRevenue,
                Name = "Försäljning",
                Description = String.Empty
            },
            new Account
            {
                AccountNo = 3001,
                OrganizationId = organizationId,
                Class = AccountClass.OperatingIncomeRevenue,
                Name = "Försäljning 25% moms Sverige",
                Description = String.Empty
            },
            new Account
            {
                AccountNo = 3002,
                OrganizationId = organizationId,
                Class = AccountClass.OperatingIncomeRevenue,
                Name = "Försäljning 12% moms Sverige",
                Description = String.Empty
            },
            new Account
            {
                AccountNo = 3003,
                OrganizationId = organizationId,
                Class = AccountClass.OperatingIncomeRevenue,
                Name = "Försäljning 6% moms Sverige",
                Description = String.Empty
            },
            new Account
            {
                AccountNo = 3040,
                OrganizationId = organizationId,
                Class = AccountClass.OperatingIncomeRevenue,
                Name = "Försäljning tjänster",
                Description = String.Empty
            },
            new Account
            {
                AccountNo = 3041,
                OrganizationId = organizationId,
                Class = AccountClass.OperatingIncomeRevenue,
                Name = "Försäljning tjänster 25% moms Sverige",
                Description = String.Empty
            },
            new Account
            {
                AccountNo = 3042,
                OrganizationId = organizationId,
                Class = AccountClass.OperatingIncomeRevenue,
                Name = "Försäljning tjänster 12% moms Sverige",
                Description = String.Empty
            },
            new Account
            {
                AccountNo = 3043,
                OrganizationId = organizationId,
                Class = AccountClass.OperatingIncomeRevenue,
                Name = "Försäljning tjänster 6% moms Sverige",
                Description = String.Empty
            },
            new Account
            {
                AccountNo = 3050,
                OrganizationId = organizationId,
                Class = AccountClass.OperatingIncomeRevenue,
                Name = "Varuförsäljning",
                Description = String.Empty
            },
            new Account
            {
                AccountNo = 3051,
                OrganizationId = organizationId,
                Class = AccountClass.OperatingIncomeRevenue,
                Name = "Försäljning varor 25% moms Sverige",
                Description = String.Empty
            },
            new Account
            {
                AccountNo = 3052,
                OrganizationId = organizationId,
                Class = AccountClass.OperatingIncomeRevenue,
                Name = "Försäljning varor 12% moms Sverige",
                Description = String.Empty
            },
            new Account
            {
                AccountNo = 3053,
                OrganizationId = organizationId,
                Class = AccountClass.OperatingIncomeRevenue,
                Name = "Försäljning varor 6% moms Sverige",
                Description = String.Empty
            },
            new Account
            {
                AccountNo = 3231,
                OrganizationId = organizationId,
                Class = AccountClass.OperatingIncomeRevenue,
                Name = "Försäljning med omvänd byggmoms",
                Description = String.Empty
            },
            new Account
            {
                AccountNo = 3550,
                OrganizationId = organizationId,
                Class = AccountClass.OperatingIncomeRevenue,
                Name = "Fakturerade resekostnader",
                Description = String.Empty
            },
            new Account
            {
                AccountNo = 3740,
                OrganizationId = organizationId,
                Class = AccountClass.OperatingIncomeRevenue,
                Name = "Öresavrundning",
                Description = String.Empty
            },
            new Account
            {
                AccountNo = 3911,
                OrganizationId = organizationId,
                Class = AccountClass.OperatingIncomeRevenue,
                Name = "Hyresintäkter",
                Description = String.Empty
            },
            new Account
            {
                AccountNo = 3921,
                OrganizationId = organizationId,
                Class = AccountClass.OperatingIncomeRevenue,
                Name = "Provisionsintäkter",
                Description = String.Empty
            },
            new Account
            {
                AccountNo = 3960,
                OrganizationId = organizationId,
                Class = AccountClass.OperatingIncomeRevenue,
                Name = "Valutakursvinster av rörelsekaraktär",
                Description = String.Empty
            },
            new Account
            {
                AccountNo = 4000,
                OrganizationId = organizationId,
                Class = AccountClass.Costs,
                Name = "Inköp av varor från Sverige",
                Description = String.Empty
            },
            new Account
            {
                AccountNo = 4010,
                OrganizationId = organizationId,
                Class = AccountClass.Costs,
                Name = "Inköp av varor och material",
                Description = String.Empty
            },
            new Account
            {
                AccountNo = 5010,
                OrganizationId = organizationId,
                Class = AccountClass.OtherOperatingExpenses1,
                Group = AccountGroup.RentedPremises,
                Name = "Lokalhyra",
                Description = String.Empty
            },
            new Account
            {
                AccountNo = 5220,
                OrganizationId = organizationId,
                Class = AccountClass.OtherOperatingExpenses1,
                Group = AccountGroup.HiredFixedAssets,
                Name = "Hyra av inventarier och verktyg",
                Description = String.Empty
            },
            new Account
            {
                AccountNo = 5410,
                OrganizationId = organizationId,
                Class = AccountClass.OtherOperatingExpenses1,
                Group = AccountGroup.ConsumableEquipmentAndSupplies,
                Name = "Förbrukningsinventarier",
                Description = "Inköp av inventarier som kostar mindre än ett halvt prisbasbelopp*, t.ex. dator, skrivare, kontorsstol"
            },
            new Account
            {
                AccountNo = 5460,
                OrganizationId = organizationId,
                Class = AccountClass.OtherOperatingExpenses1,
                Group = AccountGroup.ConsumableEquipmentAndSupplies,
                Name = "Förbrukningsmaterial",
                Description = "Material som förbrukas inom företaget, t.ex. spik, skruv"
            },
            new Account
            {
                AccountNo = 5480,
                OrganizationId = organizationId,
                Class = AccountClass.OtherOperatingExpenses1,
                Group = AccountGroup.ConsumableEquipmentAndSupplies,
                Name = "Arbetskläder & skyddsmaterial",
                Description = "Arbetskläder, Terminalglasögon Läs mer på Skatteverkets sida"
            },
            new Account
            {
                AccountNo = 5910,
                OrganizationId = organizationId,
                Class = AccountClass.OtherOperatingExpenses1,
                Group = AccountGroup.AdvertisingAndPR,
                Name = "Annonsering",
                Description = "Annonser jag köper inom Sverige"
            },
            new Account
            {
                AccountNo = 5912,
                OrganizationId = organizationId,
                Class = AccountClass.OtherOperatingExpenses1,
                Group = AccountGroup.AdvertisingAndPR,
                Name = "Annonsering EU",
                Description = "Annonser jag köper från företag in EU, t.ex. Google, Facebook när de fakturerar från Irland"
            },
            new Account
            {
                AccountNo = 6212,
                OrganizationId = organizationId,
                Class = AccountClass.OtherOperatingExpenses2,
                Name = "Mobiltelefon",
                Description = "Telefonräkningen"
            },
            new Account
            {
                AccountNo = 6540,
                OrganizationId = organizationId,
                Class = AccountClass.OtherOperatingExpenses2,
                Name = "IT-tjänster",
                Description = String.Empty
            },
            new Account
            {
                AccountNo = 6541,
                OrganizationId = organizationId,
                Class = AccountClass.OtherOperatingExpenses2,
                Name = "Redovisningsprogram",
                Description = String.Empty
            },
            new Account
            {
                AccountNo = 6570,
                OrganizationId = organizationId,
                Class = AccountClass.OtherOperatingExpenses2,
                Name = "Banktjänster",
                Description = "Alla tjänster och avgifter till din bank. Viktigt att tänka på att banktjänster är momsfria"
            },
            new Account
            {
                AccountNo = 6910,
                OrganizationId = organizationId,
                Class = AccountClass.OtherOperatingExpenses2,
                Name = "Licensavgifter och royalties",
                Description = "Royalty och licensavgifter"
            },
            new Account
            {
                AccountNo = 6970,
                OrganizationId = organizationId,
                Class = AccountClass.OtherOperatingExpenses2,
                Name = "Tidningar, facklitteratur",
                Description = String.Empty
            },
            new Account
            {
                AccountNo = 7010,
                OrganizationId = organizationId,
                Class = AccountClass.PersonnelCosts,
                Name = "Löner till kollektivanställda",
                Description = String.Empty
            },
            new Account
            {
                AccountNo = 7210,
                OrganizationId = organizationId,
                Class = AccountClass.PersonnelCosts,
                Name = "Löner till tjänstemän",
                Description = String.Empty
            },
            new Account
            {
                AccountNo = 7220,
                OrganizationId = organizationId,
                Class = AccountClass.PersonnelCosts,
                Name = "Löner till företagsledare",
                Description = String.Empty
            },
            new Account
            {
                AccountNo = 7332,
                OrganizationId = organizationId,
                Class = AccountClass.PersonnelCosts,
                Name = "Bilersättningar, skattepliktiga",
                Description = String.Empty
            },
            new Account
            {
                AccountNo = 7510,
                OrganizationId = organizationId,
                Class = AccountClass.PersonnelCosts,
                Name = "Arbetsgivaravgifter",
                Description = String.Empty
            },
            new Account
            {
                AccountNo = 7610,
                OrganizationId = organizationId,
                Class = AccountClass.PersonnelCosts,
                Name = "Utbildning",
                Description = String.Empty
            },
            new Account
            {
                AccountNo = 7620,
                OrganizationId = organizationId,
                Class = AccountClass.PersonnelCosts,
                Name = "Sjuk- och hälsovård",
                Description = String.Empty
            },
            new Account
            {
                AccountNo = 7691,
                OrganizationId = organizationId,
                Class = AccountClass.PersonnelCosts,
                Name = "Personalrekrytering",
                Description = String.Empty
            },
            new Account
            {
                AccountNo = 8014,
                OrganizationId = organizationId,
                Class = AccountClass.FinancialAndOtherIncomeAndExpenses,
                Name = "Koncernbidrag",
                Description = String.Empty
            },
            new Account
            {
                AccountNo = 8310,
                OrganizationId = organizationId,
                Class = AccountClass.FinancialAndOtherIncomeAndExpenses,
                Name = "Ränteintäkter",
                Description = String.Empty
            },
            new Account
            {
                AccountNo = 8311,
                OrganizationId = organizationId,
                Class = AccountClass.FinancialAndOtherIncomeAndExpenses,
                Name = "Ränteintäkter från bank",
                Description = String.Empty
            },
            new Account
            {
                AccountNo = 8410,
                OrganizationId = organizationId,
                Class = AccountClass.FinancialAndOtherIncomeAndExpenses,
                Name = "Räntekostnader",
                Description = String.Empty
            },
            new Account
            {
                AccountNo = 8415,
                OrganizationId = organizationId,
                Class = AccountClass.FinancialAndOtherIncomeAndExpenses,
                Name = "Räntekostnader kreditinstitut",
                Description = String.Empty
            },
            new Account
            {
                AccountNo = 8420,
                OrganizationId = organizationId,
                Class = AccountClass.FinancialAndOtherIncomeAndExpenses,
                Name = "Räntekostnader för kortfristiga skulder",
                Description = String.Empty
            },
            new Account
            {
                AccountNo = 8421,
                OrganizationId = organizationId,
                Class = AccountClass.FinancialAndOtherIncomeAndExpenses,
                Name = "Räntekostnader till kreditinstitut",
                Description = String.Empty
            },
            new Account
            {
                AccountNo = 8990,
                OrganizationId = organizationId,
                Class = AccountClass.FinancialAndOtherIncomeAndExpenses,
                Name = "Resultat",
                Description = String.Empty
            },
            new Account
            {
                AccountNo = 8999,
                OrganizationId = organizationId,
                Class = AccountClass.FinancialAndOtherIncomeAndExpenses,
                Name = "Årets resultat",
                Description = String.Empty
            }
        };
    }
}