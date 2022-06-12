using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;
using RotRut.Validation;

#nullable disable

namespace RotRut.Begaran.Rut
{
    /// <remarks/>
    [Serializable()]
    [System.ComponentModel.DesignerCategory("code")]
    [XmlType(AnonymousType = true, Namespace = "http://xmls.skatteverket.se/se/skatteverket/ht/komponent/begaran/6.0")]
    [MetadataType(typeof(BegaranArende))]
    public partial class HushallBegaranArenden : BegaranArende
    {
        /// <summary>
        /// Uppgifter om utfört arbete för Rut 
        /// </summary>
        [ValidateObject]
        public HushallBegaranArendenUtfortArbete UtfortArbete
        {
            get;
            set;
        }
    }
}