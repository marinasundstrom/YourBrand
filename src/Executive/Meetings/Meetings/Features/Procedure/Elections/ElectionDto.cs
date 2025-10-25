using YourBrand.Meetings.Features;
using YourBrand.Meetings.Features.Agendas;

namespace YourBrand.Meetings.Features.Procedure.Elections;

public sealed record ElectionResultDto(string CandidateId, string CandidateName, int Votes);

public sealed record ElectionDto(
    string Id,
    ElectionState State,
    IEnumerable<ElectionCandidateDto> Candidates,
    ElectionCandidateDto? ElectedCandidate,
    MeetingFunctionDto? MeetingFunction,
    IEnumerable<ElectionResultDto> Results);
