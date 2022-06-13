using System.Xml.Serialization;
using RotRut.Validation;

#nullable disable

namespace RotRut.Begaran.Rot
{
    /// <remarks/>
    [Serializable()]
    [System.ComponentModel.DesignerCategory("code")]
    [XmlType(AnonymousType = true, Namespace = "http://xmls.skatteverket.se/se/skatteverket/ht/komponent/begaran/6.0")]
    public partial class RotBegaranArendenUtfortArbete
    {
        /// <remarks/>
        [ValidateObject]
        public RotBegaranArendenUtfortArbeteBygg Bygg
        {
            get;
            set;
        }

        /// <remarks/>
        [ValidateObject]
        public RotBegaranArendenUtfortArbeteEl El
        {
            get;
            set;
        }

        /// <remarks/>
        [ValidateObject]
        public RotBegaranArendenUtfortArbeteGlasPlatarbete GlasPlatarbete
        {
            get;
            set;
        }

        /// <remarks/>
        [ValidateObject]
        public RotBegaranArendenUtfortArbeteMarkDraneringarbete MarkDraneringarbete
        {
            get;
            set;
        }

        /// <remarks/>
        [ValidateObject]
        public RotBegaranArendenUtfortArbeteMurning Murning
        {
            get;
            set;
        }

        /// <remarks/>
        [ValidateObject]
        public RotBegaranArendenUtfortArbeteMalningTapetsering MalningTapetsering
        {
            get;
            set;
        }

        /// <remarks/>
        [ValidateObject]
        public RotBegaranArendenUtfortArbeteVvs Vvs
        {
            get;
            set;
        }
    }
}