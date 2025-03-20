using StoreWeb;

namespace BlazorApp.Cart;

public sealed class CartService(ICartClient client) : ICartService
{
    private readonly List<CartItem> _items = new();

    public async Task InitializeAsync()
    {
        _items.Clear();
        _items.AddRange(await GetCartItemsAsync());

        CartUpdated?.Invoke(this, EventArgs.Empty);
    }

    public async Task<IEnumerable<CartItem>> GetCartItemsAsync(CancellationToken cancellationToken = default)
    {
        var cart = await client.GetCartAsync(cancellationToken);
        return cart.Items!.Select(x => new CartItem(x.Id!, x.Name!, x.Image!, x.ProductId, x.ProductHandle, x.Description!, (decimal)x.Price, x.VatRate, (decimal?)x.RegularPrice, x.DiscountRate, (int)x.Quantity, x.Data));
    }

    public async Task AddCartItem(string name, string? image, long? productId, string? productHandle, string description, decimal price, decimal? regularPrice, int quantity, string? data = null)
    {
        var ci = await client.AddCartItemAsync(new AddCartItemRequest
        {
            ProductId = productId,
            Quantity = quantity,
            Data = data
        });

        var cartItem = _items.FirstOrDefault(x => x.ProductId == productId);

        if (cartItem is not null)
        {
            cartItem.Quantity += quantity;
        }
        else
        {
            cartItem = new CartItem(ci.Id, name, image, productId, productHandle, description, price, ci.VatRate, regularPrice, ci.DiscountRate, quantity, data);

            _items.Add(cartItem);
        }

        CartUpdated?.Invoke(this, EventArgs.Empty);
    }

    public async Task UpdateCartItemQuantity(string cartItemId, int quantity)
    {
        await client.UpdateCartItemQuantityAsync(cartItemId, new UpdateCartItemQuantityRequest { Quantity = quantity });

        var cartItem = _items.FirstOrDefault(x => x.Id == cartItemId);

        if (cartItem is not null)
        {
            cartItem.Quantity = quantity;
        }

        CartUpdated?.Invoke(this, EventArgs.Empty);
    }

    public async Task RemoveCartItem(string cartItemId)
    {
        await client.RemoveCartItemAsync(cartItemId);

        var cartItem = _items.FirstOrDefault(x => x.Id == cartItemId);

        if (cartItem is not null)
        {
            _items.Remove(cartItem);
        }

        CartUpdated?.Invoke(this, EventArgs.Empty);
    }

    public async Task UpdateCartItem(string? cartItemId, int quantity, string? data)
    {
        await UpdateCartItemQuantity(cartItemId, quantity);

        await client.UpdateCartItemDataAsync(cartItemId, new UpdateCartItemDataRequest()
        {
            Data = data
        });
    }

    public IReadOnlyCollection<CartItem> Items => _items;

    public async Task Clear(bool server = true)
    {
        if(server) 
        {
            await client.ClearCartAsync();
        }

        _items.Clear();

        CartUpdated?.Invoke(this, EventArgs.Empty);
    }

    public event EventHandler? CartUpdated;
}