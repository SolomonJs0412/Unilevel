using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Unilever.v1.Models.Http.HttpReq
{
    public class TaskUpdateReq
    {
        public string TaskName { get; set; } = string.Empty;
        public string PlanName { get; set; } = string.Empty;
        public int UserCd { get; set; }
        public int PlanCd { get; set; }
        public string TaskDescription { get; set; } = string.Empty;
        public List<int>? Resources { get; set; }
        public DateTime StartDay { get; set; }
        public DateTime EndDay { get; set; }
        public int UserAssigned { get; set; }
        public string Status { get; set; } = "New";
    }
}