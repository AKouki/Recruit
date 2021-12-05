using System.ComponentModel.DataAnnotations;

namespace Recruit.Shared.ViewModels
{
    public class ScheduleInterviewViewModel
    {
        [Required]
        public int ApplicantId { get; set; }
        [Required]
        public DateTime Date { get; set; } = DateTime.Now;
        [Required]
        public DateTime Time { get; set; }
        [Required]
        public int Duration { get; set; } = 30;
    }
}
