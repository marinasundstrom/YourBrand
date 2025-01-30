using System.Text.Json;

using MediatR;

using YourBrand.Sales;
using YourBrand.StoreFront.API.Features.Cart;

namespace YourBrand.StoreFront.API.Features.Checkout;

public sealed record Checkout(
    BillingDetailsDto BillingDetails,
    ShippingDetailsDto ShippingDetails)
    : IRequest
{
    sealed class Handler(
        YourBrand.Sales.IOrdersClient ordersClient,
        YourBrand.Carts.ICartsClient cartsClient,
        //YourBrand.Inventory.IItemsClient productsClient,
        YourBrand.Inventory.Client.IWarehouseItemsClient warehouseItemsClient,
        YourBrand.Catalog.IProductsClient productsClient2,
        //ICartHubService cartHubService,
        IConfiguration configuration,
        IUserContext userContext) : IRequestHandler<Checkout>
    {
        public async Task Handle(Checkout request, CancellationToken cancellationToken)
        {
            var customerId = userContext.CustomerNo;
            var clientId = userContext.ClientId;

            string tag = customerId is null ? $"cart-{clientId}" : $"cart-{customerId}";

            var cart = await cartsClient.GetCartByTagAsync(tag, cancellationToken);

            var items = new List<CreateOrderItem>();

            await CreateOrderItems(cart, items, cancellationToken);

            const int OrderStatusOpen = 2;

            await ordersClient.CreateOrderAsync(configuration["OrganizationId"]!, new CreateOrderRequest()
            {
                Status = OrderStatusOpen,
                Customer = new SetCustomer
                {
                    Id = customerId?.ToString()
                },
                BillingDetails = new BillingDetails
                {
                    FirstName = request.BillingDetails.FirstName,
                    LastName = request.BillingDetails.LastName,
                    Ssn = request.BillingDetails.SSN,
                    Email = request.BillingDetails.Email,
                    PhoneNumber = request.BillingDetails.PhoneNumber,
                    Address = Map(request.BillingDetails.Address)
                },
                ShippingDetails = new ShippingDetails
                {
                    FirstName = request.ShippingDetails.FirstName,
                    LastName = request.ShippingDetails.LastName,
                    CareOf = request.ShippingDetails.CareOf,
                    Address = Map(request.ShippingDetails.Address)
                },
                Items = items.ToList()
            }, cancellationToken);

            await UpdateInventory(cart, cancellationToken);

            //await cartsClient.CheckoutAsync(cart.Id);
            await cartsClient.ClearCartAsync(cart.Id);
        }

        private async Task UpdateInventory(Carts.Cart cart, CancellationToken cancellationToken)
        {
            var productIds = cart.Items
                .Where(x => x.ProductId is not null)
                .Select(x => x.ProductId.GetValueOrDefault())
                .Distinct();

            var products = await productsClient2.GetProductsByIdsAsync(configuration["OrganizationId"]!, productIds, null, null, cancellationToken);

            foreach (var item in cart.Items)
            {
                var product = products.FirstOrDefault(x => x.Id == item.ProductId);

                if (product?.Sku is null)
                {
                    continue;
                }

                try
                {
                    await warehouseItemsClient.ReserveItemsAsync("main-warehouse", product.Sku, new YourBrand.Inventory.Client.ReserveItems() { Quantity = (int)item.Quantity });
                }
                catch (Exception)
                {
                    Console.WriteLine("Failed to reserve item");
                }
            }
        }

        private async Task CreateOrderItems(Carts.Cart cart, List<CreateOrderItem> items, CancellationToken cancellationToken)
        {
            var productIds = cart.Items
                .Where(x => x.ProductId is not null)
                .Select(x => x.ProductId.GetValueOrDefault())
                .Distinct();

            var products = await productsClient2.GetProductsByIdsAsync(configuration["OrganizationId"]!, productIds, null, null, cancellationToken);

            foreach (var cartItem in cart.Items)
            {
                var product = products.FirstOrDefault(x => x.Id == cartItem.ProductId);

                if (product is null)
                {
                    throw new Exception("Product not found");
                }

                var options = JsonSerializer.Deserialize<IEnumerable<Option>>(cartItem.Data!, new JsonSerializerOptions
                {
                    DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
                })!;

                decimal price = product.Price;

                price += CalculatePrice(product, options);

                List<string> optionTexts = new List<string>();

                foreach (var option in options)
                {
                    var opt = product.Options.FirstOrDefault(x => x.Option.Id == option.Id);

                    if (opt is not null)
                    {
                        if (option.OptionType == 0)
                        {
                            var isSelected = option.IsSelected.GetValueOrDefault();

                            if (!isSelected && isSelected != opt.Option.IsSelected)
                            {
                                optionTexts.Add($"No {option.Name}");

                                continue;
                            }

                            if (isSelected)
                            {
                                if (option.Price is not null)
                                {
                                    optionTexts.Add($"{option.Name} (+{option.Price?.ToString("c")})");
                                }
                                else
                                {
                                    optionTexts.Add(option.Name);
                                }
                            }
                        }
                        else if (option.SelectedValueId is not null)
                        {
                            var value = opt.Option.Values.FirstOrDefault(x => x.Id == option.SelectedValueId);

                            if (value?.Price is not null)
                            {
                                optionTexts.Add($"{value.Name} (+{value.Price?.ToString("c")})");
                            }
                            else
                            {
                                optionTexts.Add(value!.Name);
                            }
                        }
                        else if (option.NumericalValue is not null)
                        {
                            optionTexts.Add($"{option.NumericalValue} {option.Name}");
                        }
                    }
                }

                /*
                var price = item.Price
                    + options
                    .Where(x => x.IsSelected.GetValueOrDefault() || x.SelectedValueId is not null)
                    .Select(x => x.Price.GetValueOrDefault() + (x.Values.FirstOrDefault(x3 => x3.Id == x?.SelectedValueId)?.Price ?? 0))
                    .Sum();
                    */

                items.Add(new CreateOrderItem
                {
                    Description = product.Name,
                    ItemId = cartItem.ProductId?.ToString(),
                    Notes = string.Join(", ", optionTexts),
                    UnitPrice = price,
                    RegularPrice = cartItem.RegularPrice,
                    VatRate = cartItem.VatRate,
                    Quantity = cartItem.Quantity,
                    Discount = cartItem.RegularPrice is null ? null : cartItem.Price - cartItem.RegularPrice.GetValueOrDefault()
                });
            }
        }

        private YourBrand.Sales.Address Map(AddressDto address)
        {
            return new()
            {
                Street = address.Thoroughfare,
                AddressLine2 = address.SubPremises,
                PostalCode = address.PostalCode,
                City = address.Locality,
                StateOrProvince = address.AdministrativeArea,
                Country = address.Country
            };
        }

        private static decimal CalculatePrice(YourBrand.Catalog.Product item, IEnumerable<Option>? options)
        {
            decimal price = 0;

            foreach (var option in options!
                .Where(x => x.IsSelected.GetValueOrDefault() || x.SelectedValueId is not null))
            {
                var o = item.Options.FirstOrDefault(x => x.Option.Id == option.Id);
                if (o is not null)
                {
                    if (option.IsSelected.GetValueOrDefault())
                    {
                        price += option.Price.GetValueOrDefault();
                    }
                    else if (option.SelectedValueId is not null)
                    {
                        var sVal = o.Option.Values.FirstOrDefault(x => x.Id == option.SelectedValueId);
                        if (sVal is not null)
                        {
                            price += sVal.Price.GetValueOrDefault();
                        }
                    }
                }
            }

            return price;
        }
    }
}