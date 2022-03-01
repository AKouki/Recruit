using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Recruit.Shared.ViewModels
{
    public class EmailItemViewModel
    {
        public int Id { get; set; }
        [Required]
        public UserViewModel? Sender { get; set; }
        [Required]
        public string? Body { get; set; }
        [Required]
        public DateTime? SentDate { get; set; }
    }
}
