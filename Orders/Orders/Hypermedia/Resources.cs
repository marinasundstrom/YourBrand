namespace YourBrand.Orders.Hypermedia;

using System.Runtime.Serialization;

using Newtonsoft.Json;

using NJsonSchema.Converters;

//using YourBrand.Orders.Models;

[JsonConverter(typeof(JsonInheritanceConverter), "_type")]
[KnownType(typeof(Resource<>))]
public abstract class Resource
{
    [JsonProperty("_links")]
    public Dictionary<string, Link> Links { get; set; } = new Dictionary<string, Link>();
}

//[KnownType(typeof(Receipt))]
//[KnownType(typeof(ReceiptItem))]
public abstract class Resource<TEmbedded> : Resource
    where TEmbedded : class
{
    [JsonProperty("_embedded", NullValueHandling = NullValueHandling.Ignore)]
    public TEmbedded? Embedded { get; set; }
}

public class Link
{
    [JsonProperty("href")]
    public string Href { get; set; } = null!;

    [JsonProperty("method", NullValueHandling = NullValueHandling.Ignore)]
    public string? Method { get; set; }

    [JsonProperty("hrefLang", NullValueHandling = NullValueHandling.Ignore)]
    public string? HrefLang { get; set; }

    [JsonProperty("title", NullValueHandling = NullValueHandling.Ignore)]
    public string? Title { get; set; }

    [JsonProperty("templated", DefaultValueHandling = DefaultValueHandling.Ignore)]
    public bool Templated { get; set; }
}