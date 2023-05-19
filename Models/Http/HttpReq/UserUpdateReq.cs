using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Unilever.v1.Models.Http.HttpReq
{
    public class UserUpdateReq
    {
        public String Name { get; set; } = String.Empty;
        public string? PhoneNumber { get; set; }

        public string? Address { get; set; }
    }
}