using YourBrand.Meetings.Features.Agendas;

namespace YourBrand.Meetings.Features.Procedure.Elections;

public static partial class Mappings
{
    public static ElectionSessionDto ToDto(this Domain.Entities.Election electionSession) => new(electionSession.Id, electionSession.State, electionSession.Candidates.Select(x => x.ToDto()), electionSession.ElectedCandidate?.ToDto());
}