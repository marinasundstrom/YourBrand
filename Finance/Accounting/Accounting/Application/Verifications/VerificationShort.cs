namespace Accounting.Application.Verifications;

public class VerificationShort
{
    public int Id { get; set; }

    public DateTime Date { get; set; }

    public string Description { get; set; } = null!;
}