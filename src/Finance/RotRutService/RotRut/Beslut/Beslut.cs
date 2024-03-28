using System.Text.Json.Serialization;

namespace RotRut.Beslut
{

    public partial class Beslut
    {
        [JsonPropertyName("namn")]
        public string Namn { get; set; }

        [JsonPropertyName("referensnummer")]
        public string Referensnummer { get; set; }

        [JsonPropertyName("arenden")]
        public Arenden[] Arenden { get; set; }
    }
}
