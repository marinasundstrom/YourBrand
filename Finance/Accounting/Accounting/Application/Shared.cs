using System;
namespace YourBrand.Accounting.Application;

public static class Shared
{
    public static string? GetAttachmentUrl(string attachment)
    {
        if (attachment is null) return null;

        return $"https://localhost:8080/content/attachments/{attachment}";
    }
}