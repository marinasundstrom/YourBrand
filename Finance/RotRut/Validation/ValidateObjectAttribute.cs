using System.Collections;
using System.ComponentModel.DataAnnotations;

namespace RotRut.Validation
{
    public class ValidateObjectAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null)
            {
                return ValidationResult.Success;
            }

            if (value is IEnumerable enumerable)
            {
                return ValidateEnumerable(enumerable);
            }
            else
            {
                var results = new List<ValidationResult>();
                var context = new ValidationContext(value, null, null);

                Validator.TryValidateObject(value, context, results, true);

                if (results.Count != 0)
                {
                    var compositeResults = new CompositeValidationResult(value, String.Format("Validation for {0} failed.", validationContext.DisplayName));
                    results.ForEach(compositeResults.AddResult);

                    return compositeResults;
                }

                return ValidationResult.Success;
            }
        }

        private static ValidationResult ValidateEnumerable(IEnumerable enumerable)
        {
            var validationContext = new ValidationContext(enumerable);

            var compositeResults = new CompositeValidationResult(enumerable, String.Format("Validation for {0} failed.", validationContext.DisplayName));

            foreach (object item in enumerable)
            {
                var compositeResults2 = new CompositeValidationResult(item, String.Format("Validation for {0} failed.", validationContext.DisplayName));

                var results = new List<ValidationResult>();
                var context = new ValidationContext(item, null, null);

                Validator.TryValidateObject(item, context, results, true);

                if (results.Count != 0)
                {
                    results.ForEach(compositeResults2.AddResult);
                }

                compositeResults.AddResult(compositeResults2);
            }

            if (compositeResults.Results.Any())
            {
                return compositeResults;
            }

            return ValidationResult.Success;
        }
    }
}