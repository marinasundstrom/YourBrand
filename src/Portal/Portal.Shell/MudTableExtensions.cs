using System.Reflection;

using MudBlazor;

namespace YourBrand.Portal;

public static class MudTableExtensions
{
    public static TableData<T> GetServerData<T>(this MudTable<T> table)
    {
        Console.WriteLine("Foo: " + string.Join(", ", typeof(MudTable<T>).GetFields(BindingFlags.NonPublic | BindingFlags.Instance).Select(x => x.Name)));

        var field = typeof(MudTable<T>).GetField("_serverData", BindingFlags.NonPublic | BindingFlags.Instance);

        if (field == null)
            throw new InvalidOperationException("Field '_serverData' not found.");

        return (TableData<T>)field.GetValue(table)!;
    }
}