﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Recruit.Shared.ViewModels
{
    public class UpdateStatusViewModel
    {
        [Required]
        public int ApplicantId { get; set; }
        [Required]
        public int StageId { get; set; }
    }
}
