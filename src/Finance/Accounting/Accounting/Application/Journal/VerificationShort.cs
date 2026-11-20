namespace YourBrand.Accounting.Application.Journal;

public class VerificationShort
{
    public int Id { get; set; }

    public DateTimeOffset Date { get; set; }

    public string Description { get; set; } = null!;
}