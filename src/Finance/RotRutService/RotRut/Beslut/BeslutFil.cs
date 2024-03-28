using System.Text.Json.Serialization;

namespace RotRut.Beslut
{
    public partial class BeslutFil
    {
        [JsonPropertyName("version")]
        //[JsonConverter(typeof(ParseStringConverter))]
        public long Version { get; set; }

        [JsonPropertyName("utforare")]
        public string Utforare { get; set; }

        [JsonPropertyName("beslut")]
        public Beslut[] Beslut { get; set; }
    }
}
