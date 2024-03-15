using System.Text.Json;

using BlazorApp.Cart;
using BlazorApp.ProductCategories;
using BlazorApp.Products;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.JSInterop;

namespace Client.Products
{
    public partial class ProductPage
    {
        int quantity = 1;
        readonly string currency = "SEK";
        ProductInfo? productInfo;

        [Parameter]
        public string Id { get; set; } = default!;

        [Parameter]
        public string? VariantId { get; set; } = default!;

        [Parameter]
        [SupplyParameterFromQuery(Name = "cartItemId")]
        public string? CartItemId { get; set; }

        [Parameter]
        [SupplyParameterFromQuery(Name = "d")]
        public string? Data { get; set; }

        ProductViewModel? productViewModel;

        private PersistingComponentStateSubscription persistingSubscription;

        protected override async Task OnInitializedAsync()
        {
            NavigationManager.LocationChanged += OnLocationChanged;

            await Load();
        }

        private async Task Load()
        {
            persistingSubscription =
            ApplicationState.RegisterOnPersisting(PersistItems);

            if (!ApplicationState.TryTakeFromJson<ProductViewModel>(
            "productViewModel", out var restored))
            {
                var pwm = new ProductViewModel(ProductsService);
                await pwm.Initialize(Id, VariantId);
                productViewModel = pwm;
            }
            else
            {
                productViewModel = restored!;
                productViewModel.SetClient(ProductsService);
            }

            productInfo = new ProductInfo(productViewModel.Name, productViewModel.Image, productViewModel.ProductId,
            productViewModel.Id, productViewModel.Description,
            (decimal)productViewModel.Price, (decimal?)productViewModel.RegularPrice);

            if (!RenderingContext.IsPrerendering)
            {
                if (CartItemId is not null)
                {
                    var items = await CartService.GetCartItemsAsync();
                    var item = items.First(x => x.Id == CartItemId);
                    quantity = item.Quantity;

                    if (item.Data is not null)
                    {
                        Data = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(item.Data));
                    }
                }
            }

            if (Data is not null)
            {
                var str = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(Data));
                Deserialize(str);
            }

            /*
            if (!RenderingContext.IsPrerendering)
            {
                _ = ProductViewed();
            }
            */
        }

        private async void OnLocationChanged(object? sender, LocationChangedEventArgs e)
        {
            await Load();

            StateHasChanged();
        }

        private async Task ProductViewed()
        {
            /*
            await AnalyticsService.RegisterEvent(new EventData
            {
            EventType = EventType.ProductViewed,
            Data = new Dictionary<string, object>
            {
            { "productId", productViewModel.Variant?.Id ?? productViewModel.Product!.Id },
            { "name", productViewModel.Name },
            { "isEdit", CartItemId is not null }
            }
            });
            */
        }

        async Task UpdateVariant()
        {
            await productViewModel!.UpdateVariant();

            //InvokeAsync(StateHasChanged);

            await UpdateUrl();

            _ = ProductViewed();
        }

        async Task SelectVariant(Product v)
        {
            var oldVariant = productViewModel!.Variant;

            if (oldVariant?.Id != v.Id)
            {
                await productViewModel!.SelectVariant(v!);

                await UpdateUrl();

                _ = ProductViewed();
            }
        }

        async Task UpdateUrl()
        {
            string data = Serialize();
            data = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(data));

            //await JS.InvokeVoidAsync("skipScroll");

            System.Text.StringBuilder sb = new();
            sb.Append($"/products/{Id}");

            if (productViewModel!.VariantId is not null)
            {
                sb.Append($"/{productViewModel.VariantId}");
            }

            if (data is not null)
            {
                sb.Append($"?d={data}");
            }

            if (CartItemId is not null)
            {
                sb.Append($"&cartItemId={CartItemId}");
            }

            await JSRuntime.InvokeVoidAsync("changeUrl", sb.ToString());
        }

        private Task PersistItems()
        {
            ApplicationState.PersistAsJson("productViewModel", productViewModel);

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            NavigationManager.LocationChanged -= OnLocationChanged;
            persistingSubscription.Dispose();
        }

        static IEnumerable<ProductCategoryParent> IterateCategories(ProductCategoryParent
        category)
        {
            if (category.Parent is not null)
            {
                yield return IterateCategories(category.Parent).First();
            }

            yield return category;
        }
        string Serialize()
        {
            return JsonSerializer.Serialize(productViewModel!.GetData(), new JsonSerializerOptions
            {
                DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
            });
        }

        void Deserialize(string str)
        {
            var options = JsonSerializer.Deserialize<IEnumerable<ProductViewModel.Option>>(str, new JsonSerializerOptions
            {
                DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
            });

            productViewModel!.LoadData(options!);
        }

        bool isAddingItem;
        bool isUpdatingItem;

        async Task AddItemToCart()
        {
            try
            {
                isAddingItem = true;

                var product = productViewModel.Product;

                var productId = (productViewModel?.Variant?.Id ?? productViewModel?.Product?.Id);

                await CartService.AddCartItem(product.Name, product.Image.Url, productId, product.Handle, product.Description,
                    productViewModel.Total, product.RegularPrice, quantity, Serialize());

                ToastService.ShowInfo($"{productViewModel.Name} was added to your basket");
            }
            catch (Exception exc)
            {
                ToastService.ShowInfo("Failed to add item");
            }
            finally
            {
                isAddingItem = false;
            }
        }

        async Task UpdateCartItem()
        {
            try
            {
                isUpdatingItem = true;

                var product = productViewModel.Product;

                await CartService.UpdateCartItem(CartItemId, quantity, Serialize());

                //hasAddedToCart = true;

                ToastService.ShowInfo($"{productViewModel.Name} was updated in your basket");
            }
            catch (Exception exc)
            {
                ToastService.ShowInfo("Failed to update item");
            }
            finally
            {
                isUpdatingItem = false;
            }
        }
    }
}