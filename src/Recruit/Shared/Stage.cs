using System.ComponentModel.DataAnnotations;

namespace Recruit.Shared
{
    public class Stage
    {
        public int Id { get; set; }
        [Required]
        public string? Name { get; set; }
        [Required]
        public Job? Job { get; set; }
        public ICollection<Applicant>? Applicants { get; set; }
    }
}
