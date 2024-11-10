using YourBrand.Meetings.Features.Agendas;

namespace YourBrand.Meetings.Features.Procedure.Elections;

public sealed record ElectionSessionDto(string Id, ElectionState State, IEnumerable<ElectionCandidateDto> Candidates, ElectionCandidateDto? ElectedCandidate);
