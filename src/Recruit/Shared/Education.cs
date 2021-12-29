using Recruit.Shared.Validators;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Recruit.Shared
{
    public class Education : IValidatableObject
    {
        public int Id { get; set; }
        [Required]
        public string? School { get; set; }
        public string? Degree { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (StartDate != null && EndDate != null && StartDate > EndDate)
            {
                yield return new ValidationResult($"The {nameof(EndDate)} must be later than {nameof(StartDate)}.", new[] { nameof(EndDate) });
            }
        }
    }
}
