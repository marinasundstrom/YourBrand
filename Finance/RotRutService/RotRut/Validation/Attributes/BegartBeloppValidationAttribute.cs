using System.ComponentModel.DataAnnotations;
using RotRut.Begaran;

namespace RotRut.Validation.Attributes
{
    public class BegartBeloppValidationAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            decimal begartBelopp = (int)value;
            var foo = (BegaranArende)validationContext.ObjectInstance;
            if(begartBelopp > foo.BetaltBelopp) 
            {
                return new ValidationResult("Begärt belopp får inte vara större än Betalt belopp.", new [] { validationContext.MemberName });
            }

            return ValidationResult.Success; 
        }
    }
}