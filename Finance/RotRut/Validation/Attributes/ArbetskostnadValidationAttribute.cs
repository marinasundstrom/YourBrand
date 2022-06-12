using System.ComponentModel.DataAnnotations;
using RotRut.Begaran;

namespace RotRut.Validation.Attributes
{
    public class ArbetskostnadValidationAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            decimal arbetskostnad = (int)value;
            var foo = (BegaranArende)validationContext.ObjectInstance;
            if(foo.BegartBelopp + foo.BetaltBelopp > arbetskostnad ) 
            {
                return new ValidationResult("Begärt belopp + Betalt belopp får inte vara större än arbetskostnaden.", new [] { validationContext.MemberName });
            }

            return ValidationResult.Success; 
        }
    }
}