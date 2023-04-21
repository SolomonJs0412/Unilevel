using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Unilever.v1.Models.RoleConf
{
    public class Role
    {
        [Key]
        public int RoleCd { get; set; }
        public String Title { get; set; } = String.Empty;
    }
}