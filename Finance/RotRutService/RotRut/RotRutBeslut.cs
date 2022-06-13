using System.Text.Json;
using System.Text.Json.Serialization;
using RotRut.Beslut;

namespace RotRut
{
    public static class RotRutBeslut
    {
        public static BeslutFil Deserialize(string str)
        {
            var options = new JsonSerializerOptions
            {
                NumberHandling = JsonNumberHandling.AllowReadingFromString |
                JsonNumberHandling.WriteAsString
            };

            return JsonSerializer.Deserialize<RotRut.Beslut.BeslutFil>(str, options);
        }

        public static string Serialize(BeslutFil besluts)
        {
            var options = new JsonSerializerOptions
            {
                NumberHandling = JsonNumberHandling.AllowReadingFromString |
                JsonNumberHandling.WriteAsString
            };

            return JsonSerializer.Serialize(besluts, options);
        }
    }
}
