using System.ComponentModel.DataAnnotations;

namespace RotRut.Validation.Attributes
{
    public class CurrencyAmountAttribute : RangeAttribute
    {
        public CurrencyAmountAttribute() : base(0, 99999999999)
        {
        }
    }
}
