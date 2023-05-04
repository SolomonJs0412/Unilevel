using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Unilever.v1.Models.Title
{
    public class Title
    {
        [Key]
        public int TitleCd { get; set; }
        public string TitleName { get; set; } = string.Empty;
        public string? TitleDescription { get; set; }
    }
}