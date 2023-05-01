using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Unilever.v1.Models.UserConf
{
    public class UserDto
    {
        [Required, EmailAddress]
        public String Email { get; set; } = String.Empty;
        public String Name { get; set; } = String.Empty;
        public string AreaCdFK { get; set; } = string.Empty;
        public string Status { get; set; } = "Active";
        [JsonProperty("role_list")]
        public string? Role { get; set; }
        public string? Reporter { get; set; } = String.Empty;
    }
}