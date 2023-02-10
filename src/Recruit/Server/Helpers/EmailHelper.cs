using Recruit.Server.Data;
using Recruit.Shared;

namespace Recruit.Server.Helpers
{
    public class EmailHelper
    {
        public static string GenerateHtmlContent(string? body, Applicant applicant, ApplicationUser user)
        {
            body = body?.Replace("{candidate_name}", $"{applicant.FirstName} {applicant.LastName}");
            body = body?.Replace("{candidate_first_name}", applicant.FirstName);
            body = body?.Replace("{candidate_last_name}", applicant.LastName);
            body = body?.Replace("{job_title}", applicant.Job?.Title);
            body = body?.Replace("{user}", user.FullName);
            body = body?.Replace("\n", "<br/>");
            return body?.Trim() ?? "";
        }
    }
}
