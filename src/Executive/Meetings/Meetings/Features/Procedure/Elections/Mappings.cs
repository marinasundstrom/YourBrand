using System.Linq;
using YourBrand.Meetings.Features;
using YourBrand.Meetings.Features.Agendas;

namespace YourBrand.Meetings.Features.Procedure.Elections;

public static partial class Mappings
{
    public static ElectionDto ToDto(this Domain.Entities.Election election)
    {
        var results = Enumerable.Empty<ElectionResultDto>();

        if (election.State == Domain.Entities.ElectionState.ResultReady)
        {
            var activeCandidates = election.Candidates.Where(c => c.WithdrawnAt is null).ToList();
            results = activeCandidates
                .Select(candidate => new ElectionResultDto(
                    candidate.Id,
                    candidate.Name,
                    election.Ballots.Count(b => b.SelectedCandidateId == candidate.Id)))
                .ToList();
        }

        return new(
            election.Id,
            election.State,
            election.Candidates.Select(x => x.ToDto()),
            election.ElectedCandidate?.ToDto(),
            election.MeetingFunction?.ToDto(),
            results);
    }
}
