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
    public static readonly AttendeeRole Member = new(1, "Member", true, true, true, "Full member with standard participation rights.");
    public static readonly AttendeeRole Alternate = new(2, "Alternate", true, true, true, "Alternate member who can step in for a full member.");
    public static readonly AttendeeRole Observer = new(3, "Observer", false, false, false, "Present without active participation.");
    public static readonly AttendeeRole Guest = new(4, "Guest", false, true, false, "Invited guest with limited rights.");
    public static readonly AttendeeRole ExternalPresenter = new(5, "External Presenter", false, true, false, "External presenter invited to share information.");

    // List of all static instances for seeding purposes
    public static readonly AttendeeRole[] AllRoles = {
        Member, Alternate, Observer, Guest, ExternalPresenter
    };
}
