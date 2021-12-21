using Recruit.Shared;
using Recruit.Shared.Enums;

namespace Recruit.Career.Models
{
    public class JobViewModel
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public JobType JobType { get; set; }
        public string? Country { get; set; }
        public string? City { get; set; }
        public Department? Department { get; set; }
        public DateTime PostDate { get; set; }
    }
}
