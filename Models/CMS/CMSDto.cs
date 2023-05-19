using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Unilever.v1.Models.CMS
{
    public class CMSDto
    {
        public string? BannerURL { get; set; }
        public string Title { get; set; } = string.Empty;
        public string HyperText { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string Status { get; set; } = "Public";
        public int UserCd { get; set; }
        public DateTime Created { get; set; }
    }
}