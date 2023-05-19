using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Unilever.v1.Models.DistributorConf
{
    public class DistributorPlan
    {
        [Key]
        public int Id { get; set; }
        public int DistributorCd { get; set; }
        public string? Plans { get; set; }

        public DistributorPlan() { }
    }
}