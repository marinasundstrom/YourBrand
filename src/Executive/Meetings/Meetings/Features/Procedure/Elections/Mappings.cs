using YourBrand.Meetings.Features.Agendas;

namespace YourBrand.Meetings.Features.Procedure.Elections;

public static partial class Mappings
{
    public static ElectionDto ToDto(this Domain.Entities.Election election) => new(election.Id, election.State, election.Candidates.Select(x => x.ToDto()), election.ElectedCandidate?.ToDto());
}