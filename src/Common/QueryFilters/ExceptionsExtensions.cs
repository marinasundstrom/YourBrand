namespace YourBrand;

public static class ExceptionsExtensions
{
    public static bool MatchQueryFilterExceptions(this InvalidOperationException exc, Type clrType)
    {
        if(exc.Message.Contains("cannot be configured as non-owned because it has already been configured as a owned"))
        {
            Console.WriteLine($"Skipping previously configured owned type: {clrType}");

            return true;
        }
        else if (exc.Message.Contains("cannot be added to the model because its CLR type has been configured as a shared type"))
        {
            Console.WriteLine($"Skipping previously configured shared type: {clrType}");

            return true;
        }

        return false;
    }
}

/* public static class EntityExtensions
{
    public static bool IsEntityEligibleForQueryFilters(this Type clrType)
    {
        if (!clrType.IsAssignableTo(typeof(IHasDomainEvents)))
        {
            Console.WriteLine($"Skipping type {clrType} because it is not implementing IHasDomainEvents.");
            return false;
        }

        return true;
    }
} */