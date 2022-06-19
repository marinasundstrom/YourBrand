
using YourBrand.RotRutService.Domain.Common;

namespace YourBrand.RotRutService.Domain.Entities;

public class RotRutRequest : AuditableEntity
{
    private readonly List<RotRutCase> _cases = new List<RotRutCase>();

    public int Id { get; private set; }

    public string Description { get; private set; } = null!;

    public RotRutRequestStatus Status { get; private set; }

    public IReadOnlyCollection<RotRutCase> Cases => _cases;

    public void AddCase(RotRutCase @case)
    {
        _cases.Add(@case);
    }
}

public enum RotRutRequestStatus
{
    Created,
    Sent,
    Confirmed
}