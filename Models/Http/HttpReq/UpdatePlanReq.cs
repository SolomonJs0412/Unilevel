using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Unilever.v1.Models.Http.HttpReq
{
    public class UpdatePlanReq
    {
        public string Name { get; set; } = string.Empty;
        public string? Images { get; set; }
    }
}