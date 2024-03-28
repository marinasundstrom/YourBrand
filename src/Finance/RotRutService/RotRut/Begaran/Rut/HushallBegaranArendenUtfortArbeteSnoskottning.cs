using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;

#nullable disable

namespace RotRut.Begaran.Rut
{
    /// <remarks/>
    [Serializable()]
    [System.ComponentModel.DesignerCategory("code")]
    [XmlType(AnonymousType = true, Namespace = "http://xmls.skatteverket.se/se/skatteverket/ht/komponent/begaran/6.0")]
    [MetadataType(typeof(TimmarMaterial))]
    public partial class HushallBegaranArendenUtfortArbeteSnoskottning : TimmarMaterial
    {

    }
}