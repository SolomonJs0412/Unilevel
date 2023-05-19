using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Unilever.v1.Models.Http.HttpReq
{
    public class TaskCreateReq
    {
        public string TaskName { get; set; } = string.Empty;
        public int UserAssigned { get; set; }
        public string TaskDescription { get; set; } = string.Empty;
        public int PlanCd { get; set; }
        public DateTime StartDay { get; set; }
        public DateTime EndDay { get; set; }
    }
}