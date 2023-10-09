using System.ComponentModel.DataAnnotations;
using System.Globalization;
namespace la_mia_pizzeria_crud_mvc.ValidationAttributes
{
    public class NoCommaAllowedAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {

            var stringValue = value?.ToString();
            if (stringValue is not null && stringValue.Contains(','))
            {
                return new ValidationResult("The field must use '.' (dot) for decimal values.");
            }
            return ValidationResult.Success;


        }
    }
}
