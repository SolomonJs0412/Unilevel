using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Unilever.v1.Models.Plan
{
    public class PlanDto
    {
        public string TimeOfPlan { get; set; } = string.Empty;
        public int DistributorCd { get; set; }
        public string Description { get; set; } = string.Empty;
        public List<string>? Invited { get; set; }
        public DateTime StartDay { get; set; }
        public DateTime EndDay { get; set; }
    }
}