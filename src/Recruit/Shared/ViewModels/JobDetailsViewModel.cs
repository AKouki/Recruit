namespace Recruit.Shared.ViewModels
{
    public class JobDetailsViewModel
    {
        public int JobId { get; set; }
        public string? JobTitle { get; set; }
        public string? Location { get; set; }
        public string? Department { get; set; }
        public string? JobType { get; set; }
        public bool Published { get; set; }
        public List<Stage> Stages { get; set; }
        public List<Applicant> Applicants { get; set; }

        public JobDetailsViewModel()
        {
            Stages = new List<Stage>();
            Applicants = new List<Applicant>();
        }

    }
}
