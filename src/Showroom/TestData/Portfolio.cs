using System;
using System.Collections.Generic;

using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace YourBrand.Showroom.TestData
{
    public partial class Portfolio2
    {
        [JsonPropertyName("projects")]
        public Project[] Projects { get; set; }
    }

    public partial class Project
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("for")]
        public string For { get; set; }

        [JsonPropertyName("startDate")]
        public DateTime? StartDate { get; set; }

        [JsonPropertyName("endDate")]
        public DateTime? EndDate { get; set; } 

        [JsonPropertyName("ongoing")]
        public bool Ongoing { get; set; } 

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("image")]
        public string Image { get; set; }

        [JsonPropertyName("links")]
        public Link[] Links { get; set; }

        [JsonPropertyName("skills")]
        public Skills Skills { get; set; }
    }

    public partial class Link
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("url")]
        public Uri Url { get; set; }
    }

    public partial class Skills
    {
        [JsonPropertyName("platforms")]
        public string[] Platforms { get; set; }

        [JsonPropertyName("programmingLanguages")]
        public string[] ProgrammingLanguages { get; set; }

        [JsonPropertyName("frameworks")]
        public string[] Frameworks { get; set; }

        [JsonPropertyName("technologies")]
        public string[] Technologies { get; set; }

        [JsonPropertyName("applications")]
        public string[] Applications { get; set; }
    }


    public partial class Portfolio2
    {
        public static Portfolio2 FromJson(string json) => JsonSerializer.Deserialize<Portfolio2>(json)!;
    }
}