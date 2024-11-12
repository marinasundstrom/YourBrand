using YourBrand.Meetings.Features.Agendas;

namespace YourBrand.Meetings.Features.Procedure.Elections;

public sealed record ElectionDto(string Id, ElectionState State, IEnumerable<ElectionCandidateDto> Candidates, ElectionCandidateDto? ElectedCandidate);
