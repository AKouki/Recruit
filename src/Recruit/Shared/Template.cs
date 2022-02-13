﻿using Recruit.Shared.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Recruit.Shared
{
    public class Template
    {
        public int Id { get; set; }
        [Required]
        public string? Name { get; set; }
        [Required]
        [MinLength(50)]
        public string? Body { get; set; }
        [Required]
        public TemplateType? TemplateType { get; set; }
    }
}
