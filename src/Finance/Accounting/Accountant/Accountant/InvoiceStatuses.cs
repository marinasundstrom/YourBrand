namespace YourBrand.Accountant;

public enum InvoiceStatuses : int
{
    Draft = 1,
    Sent,
    Paid,
    PartiallyPaid,
    Overpaid,
    Repaid,
    PartiallyRepaid,
    Reminder,
    Void
}