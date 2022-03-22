using System.Collections;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace Recruit.Shared.Validators
{
    public class MinElementsAttribute : ValidationAttribute
    {
        public int Length { get; private set; }

        public MinElementsAttribute(int length)
        {
            Length = length;
        }

        public override string FormatErrorMessage(string name)
        {
            return string.Format(CultureInfo.CurrentCulture, ErrorMessageString, name, Length);
        }

        public override bool IsValid(object? value)
        {
            if (Length < 1)
                throw new InvalidOperationException("The Length value must be greater than zero.");

            var list = value as IList;

            return list?.Count >= Length;
        }
    }
}
