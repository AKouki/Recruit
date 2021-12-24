using Recruit.Shared.Validators;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Recruit.Shared
{
    public class Experience : IValidatableObject
    {
        public int Id { get; set; }
        [Required]
        public string? Title { get; set; }
        public string? Company { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool CurrentlyWorking {  get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (CurrentlyWorking && StartDate == null)
            {
                yield return new ValidationResult($"{nameof(StartDate)} is required.", new[] {nameof(StartDate)});
            }

            if (!CurrentlyWorking)
            {
                if (StartDate == null && EndDate != null ||
                    StartDate != null && EndDate == null)
                {
                    yield return new ValidationResult($"You must select both the {nameof(StartDate)} and {nameof(EndDate)} or neither.", new[] { nameof(StartDate), nameof(EndDate) });
                }

                if (StartDate != null && EndDate != null && StartDate > EndDate)
                {
                    yield return new ValidationResult($"The {nameof(EndDate)} must be later than {nameof(StartDate)}.", new[] { nameof(EndDate) });
                }
            }
        }
    }
}
