using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Unilever.v1.Models.DistributorConf;
using Unilever.v1.Models.UserConf;

namespace Unilever.v1.Models.AreaConf
{
    public class Area
    {
        [Key]
        public int AreaCd { get; set; }
        public String? AreaName { get; set; }
        public int? AreaCode { get; set; }

        [JsonProperty("distributor_lst")]
        public string? Distributors { get; set; }

        [JsonProperty("user_lst")]
        public string? Users { get; set; }
    }
}