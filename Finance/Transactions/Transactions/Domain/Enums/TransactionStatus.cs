namespace Transactions.Domain.Enums;

[Newtonsoft.Json.JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
public enum TransactionStatus
{
    Unverified,
    Verified,
    Payback,
    Unknown,
}