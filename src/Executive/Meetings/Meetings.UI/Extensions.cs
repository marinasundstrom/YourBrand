namespace YourBrand.Meetings;

public static class Ext 
{
    public static async Task OnThrow<TException>(this Task task, Action<TException> action)
        where TException : Exception
    {
        try  
        {
            await task;
        }
        catch (TException exc) 
        {
            action(exc);
        }
    }

    public static async Task<T> OnThrow<T, TException>(this Task<T> task, Action<TException> action, T defaultValue = default!)
        where TException : Exception
    {
        try
        {
            return await task;
        }
        catch (TException exc)
        {
            action(exc);

            return defaultValue!;
        }
    }

    public static async Task OnThrow<TException>(this Task task, Func<TException, Task> action)
        where TException : Exception
    {
        try
        {
            await task;
        }
        catch (TException exc)
        {
            await action(exc);
        }
    }

    public static async Task<T> OnThrow<T, TException>(this Task<T> task, Func<TException, Task> action, T defaultValue = default!)
        where TException : Exception
    {
        try
        {
            return await task;
        }
        catch (TException exc)
        {
            await action(exc);

            return defaultValue!;
        }
    }
}