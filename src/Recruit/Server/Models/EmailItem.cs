using Recruit.Server.Data;
using Recruit.Shared;
using System.ComponentModel.DataAnnotations;

namespace Recruit.Server.Models
{
    public class EmailItem
    {
        public int Id { get; set; }
        [Required]
        public ApplicationUser? Sender { get; set; }
        [Required]
        public Applicant? Receiver { get; set; }
        [Required]
        public string? Subject { get; set; }
        [Required]
        public string? Body { get; set; }
        [Required]
        public DateTime? SentDate { get; set; }
    }
}
