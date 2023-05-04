using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Unilever.v1.Models.Title
{
    public class TitleDto
    {
        public string TitleName { get; set; } = string.Empty;
        public string? TitleDescription { get; set; }
    }
}