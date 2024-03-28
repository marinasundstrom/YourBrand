using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;
using RotRut.Validation.Attributes;

namespace RotRut.Begaran
{
    public abstract class BegaranArende
    {
        /// <summary>
        /// Köparens personnummer, obligatorisk
        /// </summary>
        [Required]
        [PersonalNumber]
        public string Kopare { get; set; }

        /// <summary>
        /// Datum för betalning, obligatorisk. Anges enligt följande exempel: 2019-07-01
        /// </summary>
        [XmlElement(DataType = "date")]
        [Required]
        public DateTime BetalningsDatum { get; set; }

        /// <summary>
        /// Pris för arbetet (arbetskostnaden), obligatorisk 
        /// </summary>
        [Required]
        [Range(2, 99999999999)]
        [ArbetskostnadValidation]
        public int PrisForArbete { get; set; }

        /// <summary>
        /// Belopp du fått betalt för arbetet, obligatorisk 
        /// </summary>    
        [Required]
        [CurrencyAmount]

        public int BetaltBelopp { get; set; }

        /// <summary>
        /// Belopp du begär, obligatorisk 
        /// </summary>
        [Required]
        [CurrencyAmount]
        [BegartBeloppValidation]
        public int BegartBelopp { get; set; }

        /// <summary>
        /// Ärendets fakturanummer
        /// </summary>
        [Required]
        [StringLength(20)]
        public string FakturaNr { get; set; }

        /// <summary>
        /// Uppgifter om övrig kostnad. Ska fyllas i om timmar eller material angetts för någon tjänst i utfört arbete
        /// </summary>
        [XmlElement("Ovrigkostnad")]
        [CurrencyAmount]
        public int? OvrigKostnad { get; set; }
    }
}