namespace YourBrand.Meetings.Domain.Entities;

public class MemberRole
{
    public int Id { get; private set; }
    public string Name { get; private set; }
    public string? Description { get; private set; }

    // Private constructor for EF Core and for creating static instances
    private MemberRole(int id, string name, string? description = null)
    {
        Id = id;
        Name = name;
        Description = description;
    }

    // Implement equality based on Id
    public override bool Equals(object? obj) =>
        obj is MemberRole other && Id == other.Id;

    public override int GetHashCode() => Id.GetHashCode();

    public static bool operator ==(MemberRole? left, MemberRole? right) =>
        Equals(left, right);

    public static bool operator !=(MemberRole? left, MemberRole? right) =>
        !Equals(left, right);

    // Static readonly instances for each member role
    public static readonly MemberRole Chairperson = new(1, "Chairperson", "Leads the board and represents it externally.");
    public static readonly MemberRole ViceChairperson = new(2, "Vice Chairperson", "Supports the chairperson and takes over in their absence.");
    public static readonly MemberRole Secretary = new(3, "Secretary", "Records meeting minutes and handles documentation.");
    public static readonly MemberRole Treasurer = new(4, "Treasurer", "Responsible for financial oversight.");
    public static readonly MemberRole Member = new(5, "Member", "Participates in board decisions.");
    public static readonly MemberRole Alternate = new(6, "Alternate", "Acts as a substitute in the absence of regular members.");

    // List of all static instances for seeding purposes
    public static readonly MemberRole[] AllRoles = {
        Chairperson, ViceChairperson, Secretary, Treasurer, Member, Alternate
    };
}
