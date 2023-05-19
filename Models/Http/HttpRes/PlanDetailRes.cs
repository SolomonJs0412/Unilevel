using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Unilever.v1.Models.UserConf;

namespace Unilever.v1.Models.Http.HttpRes
{
    public class PlanDetailRes
    {
        public string PlanName { get; set; } = "Visit Plan";
        public string? Title { get; set; }
        public string Description { get; set; } = string.Empty;
        public List<UserDto> UserAdd { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? Images { get; set; }
        public DateTime StartDay { get; set; }
        public int DistributorCd { get; set; }
    }
}