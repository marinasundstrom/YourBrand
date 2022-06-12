using System.Xml.Serialization;
using RotRut.Validation;

#nullable disable

namespace RotRut.Begaran.Rut
{
    /// <remarks/>
    [Serializable()]
    [System.ComponentModel.DesignerCategory("code")]
    [XmlType(AnonymousType = true, Namespace = "http://xmls.skatteverket.se/se/skatteverket/ht/komponent/begaran/6.0")]
    public partial class HushallBegaranArendenUtfortArbete
    {
        /// <remarks/>
        [ValidateObject]
        public HushallBegaranArendenUtfortArbeteStadning Stadning
        {
            get;
            set;
        }

        /// <remarks/>
        [ValidateObject]
        public HushallBegaranArendenUtfortArbeteKladOchTextilvard KladOchTextilvard
        {
            get;
            set;
        }

        /// <remarks/>
        [ValidateObject]
        public HushallBegaranArendenUtfortArbeteSnoskottning Snoskottning
        {
            get;
            set;
        }

        /// <remarks/>
        [ValidateObject]
        public HushallBegaranArendenUtfortArbeteTradgardsarbete Tradgardsarbete
        {
            get;
            set;
        }

        /// <remarks/>
        [ValidateObject]
        public HushallBegaranArendenUtfortArbeteBarnpassning Barnpassning
        {
            get;
            set;
        }

        /// <remarks/>
        [ValidateObject]
        public HushallBegaranArendenUtfortArbetePersonligomsorg Personligomsorg
        {
            get;
            set;
        }

        /// <remarks/>
        [ValidateObject]
        public HushallBegaranArendenUtfortArbeteFlyttjanster Flyttjanster
        {
            get;
            set;
        }

        /// <remarks/>
        [ValidateObject]
        public HushallBegaranArendenUtfortArbeteItTjanster ItTjanster
        {
            get;
            set;
        }

        /// <remarks/>
        [ValidateObject]
        public HushallBegaranArendenUtfortArbeteReparationAvVitvaror ReparationAvVitvaror
        {
            get;
            set;
        }
    }
}