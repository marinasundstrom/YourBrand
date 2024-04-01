namespace YourBrand.TimeReport.Application.Users.Absence;

public static class AbsenceHelpers
{
    public static string? GetAttachmentUrl(string? name)
    {
        if (name is null) return null;

        return name is null ? null : $"http://127.0.0.1:10000/devstoreaccount1/attachments/{name}";
    }
}