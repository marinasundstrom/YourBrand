using System;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;

namespace YourBrand.Portal;

public class CustomAuthorizationMessageHandler : AuthorizationMessageHandler
{
    public CustomAuthorizationMessageHandler(IAccessTokenProvider provider, NavigationManager navigation)
        : base(provider, navigation)
    {
        ConfigureHandler(
            authorizedUrls: new[] {
                ServiceUrls.ApiKeysServiceUrl,
                ServiceUrls.AppServiceUrl,
                ServiceUrls.CatalogServiceUrl,
                ServiceUrls.CustomersServiceUrl,
                ServiceUrls.DocumentsServiceUrl,
                //ServiceUrls.EmailServiceUrl, 
                ServiceUrls.AccountingServiceUrl,
                ServiceUrls.InvoicingServiceUrl,
                ServiceUrls.RotRutServiceUrl,
                ServiceUrls.TransactionsServiceUrl,
                ServiceUrls.HumanResourcesServiceUrl, 
                ServiceUrls.IdentityServiceUrl,
                ServiceUrls.MarketingServiceUrl,
                ServiceUrls.MessengerServiceUrl,
                ServiceUrls.NotificationsServiceUrl,
                ServiceUrls.OrdersServiceUrl,
                ServiceUrls.ShowroomServiceUrl,
                ServiceUrls.TimeReportServiceUrl,
                ServiceUrls.WarehouseServiceUrl
            },
            scopes: new[] { Scopes.MyApi });
    }
}
