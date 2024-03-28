using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace RotRut.Validation.Attributes
{
    public class PersonalNumberAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (!Regex.IsMatch((string)value, @"^(\d{10}|\d{12})$"))
            {
                return new ValidationResult($"The value of field {validationContext.DisplayName} must be in the form YYYYMMDDXXXX.");
            }

            return ValidationResult.Success;
        }
    }
}
