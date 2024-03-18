namespace YourBrand.Customers.Domain;

public static class Errors
{
    public static class Todos
    {
        public static readonly Error TodoNotFound = new Error(nameof(TodoNotFound), "Todo not found", string.Empty);
    }
}
