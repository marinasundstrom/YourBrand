namespace YourBrand.StoreFront.API.Domain.Entities;

public sealed class Cart
{
    private readonly HashSet<CartItem> _cartItems = new HashSet<CartItem>();

    public Cart(string name)
    {
        Name = name;
    }

    internal Cart(string id, string name)
    {
        Id = id;
        Name = name;
    }

    public string Id { get; private set; } = Guid.NewGuid().ToString();

    public string Name { get; set; } = default!;

    public decimal Total { get; private set; }

    public IReadOnlyCollection<CartItem> Items => _cartItems;

    public CartItem AddItem(string name, string? image, long? productId, string? productHandle, string description, decimal price, decimal? regularPrice, int quantity)
    {
        var cartItem = _cartItems.FirstOrDefault(item => item.ProductId == productId);

        if (cartItem is null)
        {
            cartItem = new CartItem(name, image, productId, productHandle, description, price, regularPrice, quantity);
            _cartItems.Add(cartItem);
            Total += cartItem.Total;
        }
        else
        {
            UpdateCartItemQuantity(cartItem.Id, (int)cartItem.Quantity + quantity);
        }

        return cartItem;
    }

    public void RemoveItem(string cartId)
    {
        var cartItem = _cartItems.FirstOrDefault(item => item.Id == cartId);

        if (cartItem is not null)
        {
            _cartItems.Remove(cartItem);

            Total -= cartItem.Total;
        }
    }

    public void UpdateCartItemQuantity(string cartId, int quantity)
    {
        var cartItem = _cartItems.FirstOrDefault(item => item.Id == cartId);

        if (cartItem is not null)
        {
            decimal oldTotal = cartItem.Total;
            Total -= oldTotal;

            cartItem.Quantity = quantity;
            Total += cartItem.Total;
        }
    }
}

public sealed class CartItem
{
    private CartItem()
    {

    }

    public CartItem(string name, string? image, long? productId, string? productHandle, string description, decimal price, decimal? regularPrice, int quantity)
    {
        Name = name;
        Image = image;
        ProductId = productId;
        ProductHandle = productHandle;
        Description = description;
        Price = price;
        RegularPrice = regularPrice;
        Quantity = quantity;

        Created = DateTimeOffset.UtcNow;
    }

    public string Id { get; private set; } = Guid.NewGuid().ToString();

    public string Name { get; set; } = default!;

    public string? Image { get; set; }

    public long? ProductId { get; set; }

    public string? ProductHandle { get; set; }

    public string Description { get; set; } = default!;

    public decimal Price { get; set; }

    public decimal? RegularPrice { get; set; }

    public double Quantity { get; set; }

    public decimal Total => Price * (decimal)Quantity;

    public DateTimeOffset Created { get; private set; }
}