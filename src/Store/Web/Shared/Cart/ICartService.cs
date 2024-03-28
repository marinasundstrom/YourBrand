namespace BlazorApp.Cart;

public interface ICartService
{
    Task InitializeAsync();

    Task<IEnumerable<CartItem>> GetCartItemsAsync(CancellationToken cancellationToken = default);

    Task AddCartItem(string name, string? image, long? productId, string? productHandle, string description, decimal price, decimal? regularPrice, int quantity, string? data = null);

    Task UpdateCartItemQuantity(string cartItemId, int quantity);

    Task RemoveCartItem(string cartItemId);

    Task UpdateCartItem(string? cartItemId, int quantity, string? data);

    IReadOnlyCollection<CartItem> Items { get; }

    Task Clear();

    event EventHandler? CartUpdated;
}

public sealed class Cart(string id, string name, IEnumerable<CartItem> items)
{
    public string Id { get; set; } = id;

    public string Name { get; set; } = name;

    public IEnumerable<CartItem> Items { get; set; } = items;
}

public sealed class CartItem(string id, string name, string? image, long? productId, string? productHandle, string description, decimal price, double? vatRate, decimal? regularPrice, double? discountRate, int quantity, string? data)
{
    public string Id { get; set; } = id;

    public string Name { get; set; } = name;

    public string? Image { get; set; } = image;

    public long? ProductId { get; set; } = productId;

    public string? ProductHandle { get; set; } = productHandle;

    public string Description { get; set; } = description;

    public decimal Price { get; set; } = price;

    public double? VatRate { get; set; } = vatRate;

    public decimal? RegularPrice { get; set; } = regularPrice;

    public double? DiscountRate { get; set; } = discountRate;

    public int Quantity { get; set; } = quantity;

    public decimal Total => Price * Quantity;

    public string? Data { get; set; } = data;
}