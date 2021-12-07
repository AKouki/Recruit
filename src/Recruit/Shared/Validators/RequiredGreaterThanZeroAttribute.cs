using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Recruit.Shared.Validators
{
    public class RequiredGreaterThanZeroAttribute : ValidationAttribute
    {
        public override bool IsValid(object? value)
        {
            if (int.TryParse(value?.ToString(), out int result))
            {
                if (result > 0)
                    return true;
            }

            return false;
        }
    }
}
