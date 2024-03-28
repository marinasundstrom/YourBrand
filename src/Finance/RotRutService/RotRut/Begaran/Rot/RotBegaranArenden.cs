using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;
using RotRut.Validation;

#nullable disable

namespace RotRut.Begaran.Rot
{
    /// <remarks/>
    [Serializable()]
    [System.ComponentModel.DesignerCategory("code")]
    [XmlType(AnonymousType = true, Namespace = "http://xmls.skatteverket.se/se/skatteverket/ht/komponent/begaran/6.0")]
    [MetadataType(typeof(BegaranArende))]
    public partial class RotBegaranArenden : BegaranArende
    {
        /// <summary>
        /// Fastighetsbeteckning, ska fyllas i om rot-avdraget avser en fastighet 
        /// </summary>
        [XmlElement("Fastighetsbeteckning")]
        public string Fastighetsbeteckning
        {
            get;
            set;
        }

        /// <summary>
        /// Lägenhetsnummer, ska fyllas i om rot-avdraget avser en lägenhet
        /// </summary>
        [XmlElement("LagenhetsNr")]
        [StringLength(25)]
        public int? LagenhetsNr
        {
            get;
            set;
        }

        /// <summary>
        /// Bostadsrättsförenings organisationsnummer, ska fyllas i om rot-avdraget avser en lägenhet 
        /// </summary>
        [XmlElement("BrfOrgNr")]
        [RegularExpression(@"^(\d{10}|\d{12})$")]
        public string BrfOrgNr
        {
            get;
            set;
        }

        /// <summary>
        /// Uppgifter om utfört arbete för Rot 
        /// </summary>
        [ValidateObject]
        public RotBegaranArendenUtfortArbete UtfortArbete
        {
            get;
            set;
        }
    }
}