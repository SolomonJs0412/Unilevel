using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Unilever.v1.Models.Http.HttpRes.User
{
    public class AllUsersRes
    {
        public int UserCd { get; set; }
        public string AreaCd { get; set; } = string.Empty;
        public String Email { get; set; } = String.Empty;
        public String Name { get; set; } = String.Empty;
        public string? Title { get; set; }
        public string Status { get; set; } = "Active";
        public string? Reporter { get; set; } = String.Empty;
    }
}