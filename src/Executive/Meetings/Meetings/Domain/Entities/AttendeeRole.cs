using YourBrand.Domain;

namespace YourBrand.Meetings.Domain.Entities;

public class AttendeeRole : IEntity
{
    public int Id { get; private set; }
    public string Name { get; private set; }
    public string? Description { get; private set; }

    // Permissions associated with each role
    public bool CanVote { get; private set; }
    public bool CanSpeak { get; private set; }
    public bool CanPropose { get; private set; }

    protected AttendeeRole() {}

    public AttendeeRole(int id, string name, bool canVote, bool canSpeak, bool canPropose, string? description = null)
    {
        Id = id;
        Name = name;
        Description = description;
        CanVote = canVote;
        CanSpeak = canSpeak;
        CanPropose = canPropose;
    }

    // Implement equality based on Id
    public override bool Equals(object? obj) =>
        obj is AttendeeRole other && Id == other.Id;

    public override int GetHashCode() => Id.GetHashCode();

    public static bool operator ==(AttendeeRole? left, AttendeeRole? right) =>
        Equals(left, right);

    public static bool operator !=(AttendeeRole? left, AttendeeRole? right) =>
        !Equals(left, right);

    // Static readonly instances for each attendee role
    public static readonly AttendeeRole Chairperson = new(1, "Chairperson", true, true, true, "Leads the meeting");
    public static readonly AttendeeRole Secretary = new(2, "Secretary", false, true, false, "Takes notes and records minutes");
    public static readonly AttendeeRole Attendee = new(3, "Attendee", true, true, true, "Active participant");
    public static readonly AttendeeRole Observer = new(4, "Observer", false, false, false, "Present without active participation");

    // List of all static instances for seeding purposes
    public static readonly AttendeeRole[] AllRoles = {
        Chairperson, Secretary, Attendee, Observer
    };
}
