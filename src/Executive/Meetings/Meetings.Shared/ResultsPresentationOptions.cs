namespace YourBrand.Meetings;

public enum ResultsDisplayMode
{
    Diagram,
    Numbers
}

public enum ResultsValueMode
{
    Votes,
    Percent
}

public sealed record VotingResultsPresentationOptions(ResultsDisplayMode DisplayMode, ResultsValueMode ValueMode);

public sealed record ElectionResultsPresentationOptions(ResultsDisplayMode DisplayMode, ResultsValueMode ValueMode);
