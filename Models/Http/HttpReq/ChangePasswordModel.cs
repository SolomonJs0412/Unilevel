using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Unilever.v1.Models.Http.HttpReq
{
    public class ChangePasswordModel
    {
        public string newPassword { get; set; } = String.Empty;
        public string oldPassword { get; set; } = String.Empty;
        public string newPasswordReType { get; set; } = String.Empty;
    }
}