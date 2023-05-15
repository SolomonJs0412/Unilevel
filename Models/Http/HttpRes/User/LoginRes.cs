using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Unilever.v1.Models.Http.HttpRes.User
{
    public class LoginRes
    {
        public string token { get; set; } = string.Empty;
        public bool isPasswordNeedChanged { get; set; }
    }
}