using Recruit.Shared.Validators;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Recruit.Shared.ViewModels
{
    public class MoveApplicantViewModel
    {
        [RequiredGreaterThanZero]
        public int ApplicantId { get; set; }
        [RequiredGreaterThanZero]
        public int JobId { get; set; }
        [RequiredGreaterThanZero]
        public int StageId { get; set; }
    }
}
