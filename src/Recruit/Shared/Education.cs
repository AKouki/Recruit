using Recruit.Shared.Validators;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Recruit.Shared
{
    public class Education
    {
        public int Id { get; set; }
        [Required]
        public string? School { get; set; }
        public string? Degree { get; set; }
        public DateTime? StartDate { get; set; }
        [DateGreaterThanOrEqualTo("StartDate", ErrorMessage = "The {0} must be later than {1}.")]
        public DateTime? EndDate { get; set; }
    }
}
