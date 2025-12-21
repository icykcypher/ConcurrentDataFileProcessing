using System.ComponentModel.DataAnnotations;

namespace MiniOrderApp.Attributes;

public class NotNegativeAttribute : ValidationAttribute
{
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
                if (value is float and < 0) return new ValidationResult("Price cannot be negative.");
                
                return ValidationResult.Success;
        }
}