using System.Runtime.InteropServices.JavaScript;
using System.Runtime.Versioning;

using BlazorApp.Cart;

using Blazored.Toast.Services;

using Microsoft.AspNetCore.Components;

namespace Client.Cart;

[SupportedOSPlatform("browser")]
public partial class CartOffCanvas
{
    bool isDeletingItem = false;
    readonly string currency = "SEK";
    IEnumerable<BlazorApp.Cart.CartItem>? cartItems;

    protected override async Task OnInitializedAsync()
    {
        CartService.CartUpdated += OnCartUpdated;

        if (RenderingContext.IsClient)
        {
            // If this component is initialized on the Basket page
            // then that page will initiate the reload.
            // This only applies to fresh navigations or reloads
            // due to DOM preservation.

            if (!NavigationManager.Uri.ToString().EndsWith("/basket"))
            {
                await CartService.InitializeAsync();
            }

            await JSHost.ImportAsync("CartOffCanvas",
            "../Cart/CartOffCanvas.razor.js");

            Init();
        }
    }

    private async void OnCartUpdated(object? sender, EventArgs eventArgs)
    {
        cartItems = await CartService.GetCartItemsAsync();
        await InvokeAsync(StateHasChanged);
    }

    async Task UpdateItem(CartItem cartItem)
    {
        NavigationManager.NavigateTo($"/products/{cartItem.ProductHandle}?cartItemId={cartItem.Id}");

        HideCartOffCanvas();

        /*
        if(cartItem.Product.Parent is null)
        {
        NavigationManager.NavigateTo($"/products/{cartItem.Product.Id}?cartItemId={cartItem.Id}");
        }
        else
        {
        NavigationManager.NavigateTo($"/products/{cartItem.Product?.Parent.Id}/{cartItem.Product.Id}?cartItemId={cartItem.Id}");
        }
        */
    }

    async Task DeleteItem(CartItem cartItem)
    {
        try
        {
            isDeletingItem = true;

            var isProductPage = NavigationManager.Uri.Contains("/products/");

            await CartService.RemoveCartItem(cartItem.Id);

            if (isProductPage
                && NavigationManager.Uri.Contains($"/{cartItem.ProductHandle}")
                && NavigationManager.Uri.Contains($"cartItemId={cartItem.Id}"))
            {
                NavigationManager.NavigateTo("/");

                HideCartOffCanvas();
            }

            if (!CartService.Items.Any())
            {
                HideCartOffCanvas();
            }
        }
        catch (Exception exc)
        {
            ToastService.ShowError("Could not delete item.");
        }
        finally
        {
            isDeletingItem = false;
        }
    }

    public void Dispose()
    {
        CartService.CartUpdated -= OnCartUpdated;
    }

    [JSImport("init", "CartOffCanvas")]
    internal static partial void Init();

    [JSImport("hideCartOffCanvas", "CartOffCanvas")]
    internal static partial void HideCartOffCanvas();
}