using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Unilever.v1.Models.SaleSUP
{
    public class SaleSUP
    {
        [Key]
        public int SaleID { get; set; }
        public string SaleSUPCd { get; set; } = string.Empty;
        public string DistributorCd { get; set; } = string.Empty;
    }
}