using YourBrand.Domain;

namespace YourBrand.Meetings.Domain.Entities;

public class MeetingFunction : IEntity
{
    public int Id { get; private set; }

    public string Name { get; private set; } = null!;

    public string? Description { get; private set; }

    protected MeetingFunction()
    {
    }

    public MeetingFunction(int id, string name, string? description = null)
    {
        Id = id;
        Name = name;
        Description = description;
    }

    public override bool Equals(object? obj) =>
        obj is MeetingFunction other && Id == other.Id;

    public override int GetHashCode() => Id.GetHashCode();

    public static bool operator ==(MeetingFunction? left, MeetingFunction? right) =>
        Equals(left, right);

    public static bool operator !=(MeetingFunction? left, MeetingFunction? right) =>
        !Equals(left, right);

    public static readonly MeetingFunction Chairperson = new(1, "Chairperson", "Leads the meeting and maintains order.");
    public static readonly MeetingFunction Secretary = new(2, "Secretary", "Records minutes and handles documentation.");
    public static readonly MeetingFunction MinuteAdjuster = new(3, "Minute Adjuster", "Reviews and adjusts meeting minutes.");
    public static readonly MeetingFunction Teller = new(4, "Teller", "Counts votes during elections.");
    public static readonly MeetingFunction Timekeeper = new(5, "Timekeeper", "Tracks speaking time and agenda timing.");
    public static readonly MeetingFunction Facilitator = new(6, "Facilitator", "Supports discussions and keeps the meeting on track.");

    public static readonly MeetingFunction[] AllFunctions =
    {
        Chairperson,
        Secretary,
        MinuteAdjuster,
        Teller,
        Timekeeper,
        Facilitator
    };
}
