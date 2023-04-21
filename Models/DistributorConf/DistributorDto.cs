using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Unilever.v1.Models.DistributorConf
{
    public class DistributorDto
    {
        public String? Name { get; set; }
        public String Address { get; set; } = String.Empty;
        public String Email { get; set; } = String.Empty;
        public String Phone { get; set; } = String.Empty;
    }
}