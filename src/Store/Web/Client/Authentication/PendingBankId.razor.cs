using System.Runtime.InteropServices.JavaScript;

namespace Client.Authentication;

public partial class PendingBankId 
{
    [JSImport("setCookie", "PendingBankId")]
    internal static partial void SetCookie(string name, string value, [JSMarshalAs<JSType.Date>] DateTime? expiresAt = null);

    [JSImport("launchBankId", "PendingBankId")]
    internal static partial void LaunchBankId(string autoStartToken, string returnUrl);
}