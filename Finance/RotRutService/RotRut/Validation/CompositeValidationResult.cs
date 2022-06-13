using System.ComponentModel.DataAnnotations;

namespace RotRut.Validation
{
    public class CompositeValidationResult : ValidationResult
    {
        private readonly List<ValidationResult> _results = new List<ValidationResult>();

        public IEnumerable<ValidationResult> Results
        {
            get
            {
                return _results;
            }
        }

        public CompositeValidationResult(object obj, string errorMessage) : base(errorMessage) 
        { 
            Object = obj;
        }

        public CompositeValidationResult(object obj, string errorMessage, IEnumerable<string> memberNames) : base(errorMessage, memberNames) 
        { 
            Object = obj;
        }

        protected CompositeValidationResult(ValidationResult validationResult) : base(validationResult) { }

        public void AddResult(ValidationResult validationResult)
        {
            _results.Add(validationResult);
        }

        public object Object { get; set; }
    }
}