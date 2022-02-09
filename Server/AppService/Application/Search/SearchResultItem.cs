
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Catalog.Application.Search;

public class SearchResultItem
{
    public string Title { get; set; } = null!;

    [JsonConverter(typeof(StringEnumConverter))]
    public SearchResultItemType ResultType { get; set; }

    public string? Description { get; set; }

    public string? Link { get; set; }

    public string? ItemId { get; set; }

    public string? UserId { get; set; }
}