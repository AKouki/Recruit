using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Recruit.Shared.ViewModels
{
    public class UpdateOrderViewModel
    {
        public int StageId { get; set; }
        public List<OrderViewModel> Items { get; set; } = new();
    }
}
