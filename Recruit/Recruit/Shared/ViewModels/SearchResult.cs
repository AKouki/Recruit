namespace Recruit.Shared.ViewModels
{
    public class SearchResult
    {
        public IEnumerable<Job> Jobs { get; set; }
        public IEnumerable<Applicant> Applicants { get; set; }

        public SearchResult()
        {
            Jobs = new List<Job>();
            Applicants = new List<Applicant>();
        }
    }
}
