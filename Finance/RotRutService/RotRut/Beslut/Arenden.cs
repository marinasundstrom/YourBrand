using System.Text.Json.Serialization;

namespace RotRut.Beslut
{
    public partial class Arenden
    {
        [JsonPropertyName("personnummer")]
        public string Personnummer { get; set; }

        [JsonPropertyName("fakturanummer")]
        //[JsonConverter(typeof(ParseStringConverter))]
        public long? Fakturanummer { get; set; }

        [JsonPropertyName("godkantBelopp")]
        public long GodkantBelopp { get; set; }
    }
}
