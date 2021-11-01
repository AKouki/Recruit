using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Recruit.Shared
{
    public class Attachment
    {
        public int Id { get; set; }
        [Required]
        public string? FileName {  get; set; }
        [Required]
        public string? FilePath { get; set; }

        public int ApplicantId { get; set; }
        [Required]
        public Applicant? Applicant { get; set; }
    }
}
