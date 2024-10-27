using System.ComponentModel;

namespace YourBrand.Sales.Domain.Enums;

[Flags]
public enum WeekDays
{
    [Description("No days")]
    None = 0,
    [Description("Sunday")]
    Sunday = 1,
    [Description("Monday")]
    Monday = 2,
    [Description("Tuesday")]
    Tuesday = 4,
    [Description("Wednesday")]
    Wednesday = 8,
    [Description("Thursday")]
    Thursday = 0x10,
    [Description("Friday")]
    Friday = 0x20,
    [Description("Saturday")]
    Saturday = 0x40
}