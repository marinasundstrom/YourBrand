using System.ComponentModel.DataAnnotations;
using RotRut.Validation.Attributes;

#nullable disable

namespace RotRut.Begaran
{
    /// <summary>
    /// Uppgifter om utfört arbete, timmar och material 
    /// </summary>
    public abstract class TimmarMaterial
    {
        /// <summary>
        /// Antal timmar
        /// </summary>
        [Required]
        [Range(0, 999)]
        public double? AntalTimmar { get; set; }

        /// <summary>
        /// Materialkostnad
        /// </summary>
        [Required]
        [CurrencyAmount]
        public decimal? Materialkostnad { get; set; }
    }
}