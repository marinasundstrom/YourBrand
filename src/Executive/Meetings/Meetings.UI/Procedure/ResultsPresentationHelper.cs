using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using MudBlazor;
using YourBrand.Meetings;

namespace YourBrand.Meetings.Procedure;

internal static class ResultsPresentationHelper
{
    public static List<ChartSeries> BuildVoteSeries(VoteBreakdown? breakdown, ResultsValueMode valueMode)
    {
        var data = BuildVoteData(breakdown, valueMode);

        return new List<ChartSeries>
        {
            new()
            {
                Name = ValueLabel(valueMode),
                Data = data
            }
        };
    }

    public static string FormatVoteValue(int count, VoteBreakdown? breakdown, ResultsValueMode valueMode)
    {
        if (breakdown is null)
        {
            return valueMode == ResultsValueMode.Percent ? "0%" : "0";
        }

        return valueMode == ResultsValueMode.Percent
            ? FormatPercent(count, TotalVotes(breakdown))
            : FormatVotes(count);
    }

    public static List<ChartSeries> BuildElectionSeries(IEnumerable<ElectionResult> results, ResultsValueMode valueMode)
    {
        var resultList = results.ToList();
        var data = BuildElectionData(resultList, valueMode);

        return new List<ChartSeries>
        {
            new()
            {
                Name = ValueLabel(valueMode),
                Data = data
            }
        };
    }

    public static string FormatElectionValue(int votes, int totalVotes, ResultsValueMode valueMode)
        => valueMode == ResultsValueMode.Percent ? FormatPercent(votes, totalVotes) : FormatVotes(votes);

    public static string ValueLabel(ResultsValueMode valueMode) => valueMode == ResultsValueMode.Percent ? "Percent" : "Votes";

    private static double[] BuildVoteData(VoteBreakdown? breakdown, ResultsValueMode valueMode)
    {
        if (breakdown is null)
        {
            return new double[] { 0d, 0d, 0d };
        }

        var totals = TotalVotes(breakdown);

        if (totals == 0)
        {
            return new double[] { 0d, 0d, 0d };
        }

        return valueMode == ResultsValueMode.Percent
            ? new[]
            {
                ToPercent(breakdown.ForVotes, totals),
                ToPercent(breakdown.AgainstVotes, totals),
                ToPercent(breakdown.AbstainVotes, totals)
            }
            : new[]
            {
                (double)breakdown.ForVotes,
                (double)breakdown.AgainstVotes,
                (double)breakdown.AbstainVotes
            };
    }

    private static double[] BuildElectionData(IEnumerable<ElectionResult> results, ResultsValueMode valueMode)
    {
        var resultList = results.ToList();
        var total = resultList.Sum(r => r.Votes);

        if (total == 0)
        {
            return resultList.Select(_ => 0d).ToArray();
        }

        return valueMode == ResultsValueMode.Percent
            ? resultList.Select(r => ToPercent(r.Votes, total)).ToArray()
            : resultList.Select(r => (double)r.Votes).ToArray();
    }

    private static int TotalVotes(VoteBreakdown breakdown) => breakdown.ForVotes + breakdown.AgainstVotes + breakdown.AbstainVotes;

    private static double ToPercent(int value, int total) => total == 0 ? 0d : (double)value / total * 100d;

    private static string FormatPercent(int value, int total)
    {
        if (total == 0)
        {
            return "0%";
        }

        var percent = (double)value / total * 100d;
        return percent.ToString("0.##" , CultureInfo.InvariantCulture) + "%";
    }

    private static string FormatVotes(int value) => value.ToString(CultureInfo.InvariantCulture);
}
