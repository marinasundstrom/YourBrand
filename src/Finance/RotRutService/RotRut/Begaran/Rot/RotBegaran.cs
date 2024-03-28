using System.Xml.Serialization;
using RotRut.Validation;

#nullable disable

namespace RotRut.Begaran.Rot
{
    /// <remarks/>
    [Serializable()]
    [System.ComponentModel.DesignerCategory("code")]
    [XmlType(AnonymousType = true, Namespace = "http://xmls.skatteverket.se/se/skatteverket/ht/komponent/begaran/6.0")]
    [XmlRoot(Namespace = "http://xmls.skatteverket.se/se/skatteverket/ht/komponent/begaran/6.0", IsNullable = false)]
    public partial class RotBegaran
    {
        /// <remarks/>
        [XmlElement("Arenden")]
        [ValidateObject]
        public RotBegaranArenden[] Arenden
        {
            get;
            set;
        }
    }
}