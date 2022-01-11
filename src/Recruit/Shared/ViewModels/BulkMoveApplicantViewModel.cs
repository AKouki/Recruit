using Recruit.Shared.Validators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Recruit.Shared.ViewModels
{
    public class BulkMoveApplicantViewModel
    {
        [RequiredGreaterThanZero]
        public int JobId { get; set; }
        [RequiredGreaterThanZero]
        public int StageId { get; set; }
        public List<int> Applicants { get; set; } = new();
    }
}
