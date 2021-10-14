﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Recruit.Shared
{
    public class UpdateStatusModel
    {
        [Required]
        public int ApplicantId { get; set; }
        [Required]
        public int StageId { get; set; }
    }
}
