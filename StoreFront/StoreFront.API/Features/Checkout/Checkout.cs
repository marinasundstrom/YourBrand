using System;
using System.Text.Json;

using MediatR;

using YourBrand.StoreFront.API.Features.Cart;

using YourBrand.Sales;

namespace YourBrand.StoreFront.API.Features.Checkout;

public sealed record Checkout(
    BillingDetailsDto BillingDetails,
    ShippingDetailsDto ShippingDetails)
    : IRequest
{
    sealed class Handler : IRequestHandler<Checkout>
    {
        private readonly YourBrand.Sales.IOrdersClient _ordersClient;
        private readonly YourBrand.Carts.ICartsClient cartsClient;
        //private readonly IItemsClient productsClient;
        private readonly YourBrand.Inventory.Client.IWarehouseItemsClient productsClient1;
        private readonly YourBrand.Catalog.IProductsClient productsClient2;
        private readonly ICurrentUserService currentUserService;
        //private readonly ICartHubService cartHubService;

        public Handler(
            YourBrand.Sales.IOrdersClient ordersClient,
            YourBrand.Carts.ICartsClient cartsClient,
            //YourBrand.Inventory.IItemsClient productsClient,
            YourBrand.Inventory.Client.IWarehouseItemsClient productsClient1,
            YourBrand.Catalog.IProductsClient productsClient2,
            //ICartHubService cartHubService,
            ICurrentUserService currentUserService)
        {
            _ordersClient = ordersClient;
            this.cartsClient = cartsClient;
            //this.productsClient = productsClient;
            //this.productsClient1 = productsClient1;
            this.productsClient2 = productsClient2;
            this.currentUserService = currentUserService;
            //this.cartHubService = cartHubService;
        }

        public async Task Handle(Checkout request, CancellationToken cancellationToken)
        {
            var customerId = currentUserService.CustomerNo;
            var clientId = currentUserService.ClientId;

            string tag = customerId is null ? $"cart-{clientId}" : $"cart-{customerId}";

            var cart = await cartsClient.GetCartByTagAsync(tag);

            var items = new List<CreateOrderItem>();

            foreach (var cartItem in cart.Items)
            {
                var product = await productsClient2.GetProductByIdAsync(cartItem.ProductId.ToString()!, cancellationToken);

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

                items.Add(new YourBrand.Sales.CreateOrderItem
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

            const int OrderStatusOpen = 2;

            await _ordersClient.CreateOrderAsync(new CreateOrderRequest()
            {
                Status = OrderStatusOpen,
                CustomerId = customerId?.ToString(),
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

            foreach (var item in items)
            {
                if(item.ItemId is null) 
                {
                    continue;
                }

                try
                {
                    await productsClient1.ReserveItemsAsync("main-warehouse", item.ItemId, new YourBrand.Inventory.Client.ReserveItems() { Quantity = (int)item.Quantity });
                }
                catch (Exception e)
                {
                }
            }

            //await cartsClient.CheckoutAsync(cart.Id);
            await cartsClient.ClearCartAsync(cart.Id);
        }

        private YourBrand.Sales.Address Map(AddressDto address)
        {
            return new()
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

