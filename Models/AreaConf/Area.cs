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
        public int AreaId { get; set; }
        public string? AreaCd { get; set; }
        public String? AreaName { get; set; }

        [JsonProperty("distributor_lst")]
        public string? Distributors { get; set; }

        [JsonProperty("user_lst")]
        public string? Users { get; set; }

        public Area() { }

        public Area(string AreaCd, string AreaName, string? Users, string? Distributors)
        {
            this.AreaCd = AreaCd;
            this.AreaName = AreaName;
            this.Users = Users;
            this.Distributors = Distributors;
        }
    }
}