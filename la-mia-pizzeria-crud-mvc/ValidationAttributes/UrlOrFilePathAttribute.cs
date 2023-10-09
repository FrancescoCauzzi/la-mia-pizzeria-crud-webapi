using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Text.RegularExpressions;

namespace la_mia_pizzeria_crud_mvc.ValidationAttributes
{


    public class UrlOrFilePathAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var stringValue = (string)value!;
            if (string.IsNullOrEmpty(stringValue))
            {
                // If the value is null or empty, validation succeeds
                return ValidationResult.Success;
            }

            bool isValidUrl = Uri.TryCreate(stringValue, UriKind.Absolute, out _);
            bool isValidFilePath = IsValidFilePath(stringValue);


            if (isValidUrl || isValidFilePath)
            {
                // If the value is a valid URL or file path, validation succeeds
                return ValidationResult.Success;
            }

            // Otherwise, validation fails
            return new ValidationResult("The input is not a valid URL or file path");


        }

        private static bool IsValidFilePath(string filePath)
        {
            /*
            char[] invalidChars = Path.GetInvalidPathChars();
            string pattern = @"^[a-zA-Z]:\\(?:[^\\/:*?""<>|\r\n]+\\)*[^\\/:*?""<>|\r\n]*$";

            // Check for invalid characters
            foreach (char invalidChar in invalidChars)
            {
                if (filePath.Contains(invalidChar))
                {
                    return false;
                }
            }

            // Check if the file path matches the pattern
            return Regex.IsMatch(filePath, pattern);
            */
            return filePath.StartsWith("/");
        }

    }

    /*
     when you apply an attribute to a class, method, or property, you can omit the "Attribute" suffix and just use the base name of the attribute class. The compiler knows to look for a class with the name you've provided, followed by "Attribute".
     */





}
