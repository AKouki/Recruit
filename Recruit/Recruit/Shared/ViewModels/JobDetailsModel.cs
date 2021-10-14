namespace Recruit.Shared.ViewModels
{
    public class JobDetailsModel
    {
        public int JobId { get; set; }
        public string? JobTitle { get; set; }
        public IEnumerable<Stage> Stages { get; set; }
        public IEnumerable<Applicant> Applicants { get; set; }

        public JobDetailsModel()
        {
            Stages = new List<Stage>();
            Applicants = new List<Applicant>();
        }

    }
}
