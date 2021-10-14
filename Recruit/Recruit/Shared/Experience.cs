using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Recruit.Shared
{
    public class Experience
    {
        public int Id { get; set; }
        [Required]
        public string? Title { get; set; }
        public string? Company { get; set; }
        public string? Summary { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool CurrentlyWorkingHere {  get; set; }
    }
}
