using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Unilever.v1.Models.Plan
{
    public class PlanDetail
    {
        [Key]
        public int Id { get; set; }
        public int PlanCd { get; set; }
        public string PlanName { get; set; } = "Visit Plan";
        public string? Images { get; set; }

        public PlanDetail() { }
    }
}