using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Recruit.Shared.Validators
{
    public class DateGreaterThanOrEqualToAttribute : ValidationAttribute
    {
        public DateGreaterThanOrEqualToAttribute(string otherAttribute)
        {
            OtherProperty = otherAttribute;
        }

        public string OtherProperty { get; set; }

        public override string FormatErrorMessage(string name)
        {
            return string.Format(CultureInfo.CurrentCulture, ErrorMessageString, name, OtherProperty);
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var otherPropertyInfo = validationContext.ObjectType.GetProperty(OtherProperty);
            if (otherPropertyInfo == null)
                return new ValidationResult($"{OtherProperty} is required.");

            var otherPropertyValue = otherPropertyInfo.GetValue(validationContext.ObjectInstance, null);

            // When we use nullable datetime (DateTime?), we want to pass the validation if both values are null.
            if (value == null && otherPropertyValue == null)
                return ValidationResult.Success;

            try
            {
                if (value == null || otherPropertyValue == null || (DateTime)value < (DateTime)otherPropertyValue)
                {
                    return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
                }
            }
            catch (Exception)
            {
                return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
            }
            
            return ValidationResult.Success;
        }
    }
}
