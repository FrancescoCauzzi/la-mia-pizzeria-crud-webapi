using System.ComponentModel.DataAnnotations;

namespace la_mia_pizzeria_crud_mvc.ValidationAttributes
{
    public class MoreThanFiveWordsAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is string)
            {
                string inputValue = (string)value;

                if (inputValue == null || inputValue.Split(' ').Length <= 5)
                {
                    return new ValidationResult("This field must contain more than 5 words");
                }

                return ValidationResult.Success;

            }

            return new ValidationResult("The field is not of type string");
        }
    }
}
