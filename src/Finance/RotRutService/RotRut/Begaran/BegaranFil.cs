using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;
using RotRut.Begaran.Rot;
using RotRut.Begaran.Rut;
using RotRut.Validation;

#nullable disable

namespace RotRut.Begaran
{
    // NOTE: Generated code may require at least .NET Framework 4.5 or .NET Core/Standard 2.0.
    /// <remarks/>
    [Serializable()]
    [System.ComponentModel.DesignerCategory("code")]
    [XmlType(AnonymousType = true, Namespace = "http://xmls.skatteverket.se/se/skatteverket/ht/begaran/6.0")]
    [XmlRoot(Namespace = "http://xmls.skatteverket.se/se/skatteverket/ht/begaran/6.0", IsNullable = false)]
    public partial class BegaranFil
    {

        /// <remarks/>
        [XmlElement(Namespace = "http://xmls.skatteverket.se/se/skatteverket/ht/komponent/begaran/6.0")]
        [Required]
        public string NamnPaBegaran
        {
            get;
            set;
        }

        /// <remarks/>
        [XmlElement("RotBegaran", Namespace = "http://xmls.skatteverket.se/se/skatteverket/ht/komponent/begaran/6.0", IsNullable = false)]
        [ValidateObject]
        public RotBegaran RotBegaran
        {
            get;
            set;
        }

        /// <remarks/>
        [XmlElement("HushallBegaran", Namespace = "http://xmls.skatteverket.se/se/skatteverket/ht/komponent/begaran/6.0", IsNullable = false)]
        [ValidateObject]
        public HushallBegaran HushallBegaran
        {
            get;
            set;
        }
    }
}