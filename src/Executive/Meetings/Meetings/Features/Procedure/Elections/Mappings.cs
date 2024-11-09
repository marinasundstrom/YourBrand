namespace YourBrand.Meetings.Features.Procedure.Elections;

public static partial class Mappings
{
    public static ElectionSessionDto ToDto(this Domain.Entities.Election electionSession) => new(electionSession.Id);
}