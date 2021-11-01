using System.ComponentModel.DataAnnotations;

namespace Recruit.Shared
{
    public class Applicant
    {
        public int Id { get; set; }

        // Personal information
        [Required]
        public string? FirstName { get; set; }
        [Required]
        public string? LastName { get; set; }
        public string? Headline { get; set; }
        public string? Summary { get; set; }
        public string? ProfilePhoto { get; set; }
        [Required]
        [EmailAddress]
        public string? Email { get; set; }
        [Required]
        [Phone]
        [MinLength(10)]
        public string? Phone { get; set; }
        public string? Address { get; set; }

        // Education/Experience
        public ICollection<Education>? Education { get; set; }
        public ICollection<Experience>? Experience { get; set; }
        public Attachment? Resume { get; set; }
        public string? Skills { get; set; }

        public DateTime ApplyDate { get; set; }
        public Interview? Interview { get; set; }
        public int JobId { get; set; }
        public Job? Job { get; set; }
        public Stage? Stage { get; set; }
    }
}
