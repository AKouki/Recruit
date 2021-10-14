using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Recruit.Shared
{
    public class Interview
    {
        public int Id { get; set; }
        public int ApplicantId { get; set; }
        [Required]
        public Applicant? Applicant { get; set; }
        [Required]
        public DateTime ScheduledAt { get; set; }
        public string? Interviewer { get; set; }
        public int Duration { get; set; }
    }
}
