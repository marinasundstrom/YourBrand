using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;

namespace RotRut
{
    public static class RotRutBegaran
    {
        static XmlSerializer serializer = new XmlSerializer(typeof(RotRut.Begaran.BegaranFil));

        public static RotRut.Begaran.BegaranFil Deserialize(Stream stream)
        {
            return (RotRut.Begaran.BegaranFil)serializer.Deserialize(stream);
        }

        public static void Serialize(Stream stream, RotRut.Begaran.BegaranFil begaran)
        {
            serializer.Serialize(stream, begaran);
        }

        public static bool Validate(RotRut.Begaran.BegaranFil begaran, ICollection<ValidationResult> results)
        {
            var validationContext = new ValidationContext(begaran);
            return Validator.TryValidateObject(begaran, validationContext, results, true);
        }

        public static bool Validate(RotRut.Begaran.BegaranArende arende, ICollection<ValidationResult> results)
        {
            var validationContext = new ValidationContext(arende);
            return Validator.TryValidateObject(arende, validationContext, results, true);
        }
    }
}
