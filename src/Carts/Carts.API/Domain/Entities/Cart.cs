using YourBrand.Tenancy;

namespace YourBrand.Carts.Domain.Entities;

public sealed class Cart : IAuditable, IHasTenant
{
    private readonly HashSet<CartItem> _cartItems = new HashSet<CartItem>();

    public Cart(string tag)
    {
        Tag = tag;
    }

    internal Cart(string id, string tag)
    {
        Id = id;
        Tag = tag;
    }

    public string Id { get; private set; } = Guid.NewGuid().ToString();

    public TenantId TenantId { get; set; }

    public string Tag { get; set; } = default!;

    public decimal Total { get; private set; }

    public IReadOnlyCollection<CartItem> Items => _cartItems;

    public DateTimeOffset Created { get; set; }
    public string? CreatedById { get; set; }
    public string? LastModifiedById { get; set; }
    public DateTimeOffset? LastModified { get; set; }

    public CartItem AddItem(string name, string? image, long? productId, string? productHandle, string description, decimal price, double? vatRate, decimal? regularPrice, double? discountRate, int quantity, string? data)
    {
        var cartItem = _cartItems.FirstOrDefault(item => item.ProductId == productId && item.Data == data);

        if (cartItem is null)
        {
            cartItem = new CartItem(name, image, productId, productHandle, description, price, vatRate, regularPrice, discountRate, quantity, data);
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

    public void UpdateCartItemQuantity(string cartItemId, int quantity)
    {
        var cartItem = _cartItems.FirstOrDefault(item => item.Id == cartItemId);

        if (cartItem is not null)
        {
            decimal oldTotal = cartItem.Total;
            Total -= oldTotal;

            cartItem.Quantity = quantity;
            Total += cartItem.Total;
        }
    }

    public void UpdateCartItemPrice(string cartItemId, decimal price)
    {
        var cartItem = _cartItems.FirstOrDefault(item => item.Id == cartItemId);

        if (cartItem is not null)
        {
            decimal oldTotal = cartItem.Total;
            Total -= oldTotal;

            cartItem.Price = price;
            Total += cartItem.Total;
        }
    }

    public void Clear()
    {
        _cartItems.Clear();
    }
}

public sealed class CartItem : IAuditable, IHasTenant
{
    private CartItem()
    {

    }

    public CartItem(string name, string? image, long? productId, string? productHandle, string description, decimal price, double? vatRate, decimal? regularPrice, double? discountRate, int quantity, string? data)
    {
        Name = name;
        Image = image;
        ProductId = productId;
        ProductHandle = productHandle;
        Description = description;
        Price = price;
        VatRate = vatRate;
        RegularPrice = regularPrice;
        DiscountRate = discountRate;
        Quantity = quantity;
        Data = data;
    }

    public string Id { get; private set; } = Guid.NewGuid().ToString();

    public TenantId TenantId { get; set; }

    public string Name { get; set; } = default!;

    public string? Image { get; set; }

    public long? ProductId { get; set; }

    public string? ProductHandle { get; set; }

    public string Description { get; set; } = default!;

    public decimal Price { get; set; }

    public double? VatRate { get; set; }

    public decimal? RegularPrice { get; set; }

    public double? DiscountRate { get; set; }

    public double Quantity { get; set; }

    public decimal Total => Price * (decimal)Quantity;

    public string? Data { get; private set; }

    //public string? Notes { get; set; }

    public void UpdateData(string? data)
    {
        Data = data;
    }

    public DateTimeOffset Created { get; set; }
    public string? CreatedById { get; set; }
    public string? LastModifiedById { get; set; }
    public DateTimeOffset? LastModified { get; set; }

}