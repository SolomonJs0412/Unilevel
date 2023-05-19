using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Unilever.v1.Models.Http.HttpReq
{
    public class CMSCreateREq
    {
        public string? BannerURL { get; set; }
        public string Title { get; set; } = string.Empty;
        public string HyperText { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int UserCd { get; set; }
        public DateTime Created { get; set; }
    }
}