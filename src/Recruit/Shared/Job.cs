using Recruit.Shared.Enums;
using System.ComponentModel.DataAnnotations;

namespace Recruit.Shared
{
    public class Job
    {
        public int Id { get; set; }
        [Required]
        public string? Title { get; set; }
        [Required]
        [MinLength(50)]
        public string? Description { get; set; }
        public int? DepartmentId { get; set; }
        public Department? Department { get; set; }

        // Job Location
        [Required]
        public string? Country { get; set; }
        [Required]
        public string? City { get; set; }

        // Contact details
        [Required]
        [Phone]
        [MinLength(10)]
        public string? ContactPhone { get; set; }
        [Required]
        [EmailAddress]
        public string? ContactEmail { get; set; }
        public string? Manager { get; set; }

        // Employment details
        public JobType JobType { get; set; }
        public JobExperience JobExperience { get; set; }
        public string? RequiredSkills { get; set; }

        public DateTime PostDate { get; set; }
        public DateTime Expires { get; set; }
        public float SalaryFrom { get; set; }
        public float SalaryTo { get; set; }

        public bool Published { get; set; }

        public ICollection<Stage>? Stages { get; set; }
        public ICollection<Applicant>? Applicants { get; set; }
    }
}
