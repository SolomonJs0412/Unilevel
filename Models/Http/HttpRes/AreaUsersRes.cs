using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Unilever.v1.Models.Http.HttpRes
{
    public class AreaUsersRes
    {
        public int UserCd { get; set; }
        public string AreaCdFK { get; set; } = string.Empty;
        public String Email { get; set; } = String.Empty;
        public String Name { get; set; } = String.Empty;
        public string Status { get; set; } = "Active";
        [JsonProperty("role_list")]
        public string? Role { get; set; }
        public string? Reporter { get; set; } = String.Empty;

        public AreaUsersRes() { }
    }
}