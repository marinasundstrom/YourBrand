namespace YourBrand.TimeReport.Application.Projects.Expenses;

public static class ExpensesHelpers
{
    public static string? GetAttachmentUrl(string? name)
    {
        if (name is null) return null;

        return name is null ? null : $"http://127.0.0.1:10000/devstoreaccount1/attachments/{name}";
    }
}