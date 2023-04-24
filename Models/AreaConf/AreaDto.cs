using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Unilever.v1.Models.AreaConf
{
    public class AreaDto
    {
        public string AreaCd { get; set; }
        public String? AreaName { get; set; }

        [JsonProperty("distributor_lst")]
        public string? Distributors { get; set; }

        [JsonProperty("user_lst")]
        public string? Users { get; set; }
    }
}